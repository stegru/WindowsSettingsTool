using SettingsHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsSettingsTool
{
    public class SettingInfo: IComponent
    {
        public string SettingId { get; private set; }
        public SettingType SettingType { get; private set; }
        public SettingItem SettingItem { get; private set; }
        protected ISettingItem setting { get; private set; }

        public string Description { get => GetString(() => this.setting.Description); }
        public bool IsEnabled { get => this.setting.IsEnabled; }
        public bool IsApplicable { get => this.setting.IsApplicable; }
        public bool IsSetByGroupPolicy { get => this.setting.IsSetByGroupPolicy; }
        public int Id { get; }

        [Browsable(false)]
        public ISite Site
        {
            get { return new SettingVerbs(this); }
            set { throw new NotImplementedException(); }
        }

        [HandleProcessCorruptedStateExceptions]
        public static string GetString(Func<object> call)
        {
            try
            {
                object ret = call();
                return (ret != null) ? ret.ToString() : "<null>";
            }
            catch (Exception e)
            {
                return (e.InnerException ?? e).Message;
            }
        }

        protected SettingInfo(string settingId)
        {
            this.SettingId = settingId;
            this.SettingItem = new SettingItem(settingId);
            this.SettingType = this.SettingItem.SettingType;

            Type myType = this.SettingItem.GetType();
            this.setting = myType.GetField("settingItem", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(this.SettingItem) as ISettingItem;

            myType.GetField("gotValue", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(this.SettingItem, true);

        }

        public static SettingInfo Get(string settingId, SettingType settingType)
        {
            // Instansiate the correct class, based on the type.
            string typeName = "WindowsSettingsTool." + settingType.ToString() + "SettingInfo";
            Type type = Type.GetType(typeName, false);
            if (type == null)
            {
                return new SettingInfo(settingId);
            }
            else
            {
                return Activator.CreateInstance(type, settingId) as SettingInfo;
            }
        }

        #region IDisposable Support
        public event EventHandler Disposed;

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    public abstract class ValueSettingInfo<TValue> : SettingInfo
        //where TValue : new()
    {
        private TValue storedValue;
        private bool loaded = false;

        public virtual TValue Value
        {
            get
            {
                if (!this.loaded)
                {
                    object value = this.SettingItem.GetValue();
                    this.storedValue = (value is TValue) ? (TValue)value : default(TValue);
                    //this.loaded = true;
                }

                return this.storedValue;
            }
            set
            {
                this.loaded = false;
                this.SettingItem.SetValue(value);
            }
        }
        protected ValueSettingInfo(string settingId) : base(settingId)
        {
        }
    }

    public class BooleanSettingInfo : ValueSettingInfo<bool>
    {
        public BooleanSettingInfo(string settingId) : base(settingId)
        {
        }
    }

    public class RangeSettingInfo : ValueSettingInfo<int>
    {
        public RangeSettingInfo(string settingId) : base(settingId)
        {
        }
    }

    public interface IPossibleValues
    {
        List<object> PossibleValues { get; }
    }

    public class ListSettingInfo : ValueSettingInfo<string>, IPossibleValues
    {

        [TypeConverter(typeof(ValueConverter))]
        public override string Value { get => base.Value; set => base.Value = value; }

        public List<object> PossibleValues { get; private set; }

        public ListSettingInfo(string settingId) : base(settingId)
        {
            try
            {
                this.PossibleValues = this.SettingItem.GetPossibleValues().ToList();
            } catch (Exception e)
            {
                this.PossibleValues = new List<object> {
                    (e.InnerException ?? e).Message
                };
        }
        }
    }

    /// <summary>
    /// Generates the drop-down values for the list setting.
    /// </summary>
    public class ValueConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            IPossibleValues inst = context.Instance as IPossibleValues;
            return (inst == null) ? base.GetStandardValuesSupported(context) : true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IPossibleValues inst = context.Instance as IPossibleValues;
            if (inst == null)
            {
                return base.GetStandardValues(context);
            }
            else
            {
                return new StandardValuesCollection(inst.PossibleValues);
            }
        }
    }

    class ActionSettingInfo : SettingInfo
    {
        public ActionSettingInfo(string settingId) : base(settingId)
        {
        }

        [Browsable(true)]
        public void Invoke()
        {
            this.SettingItem.Invoke();
        }
        
    }

    public class SettingVerbs : IMenuCommandService, ISite
    {
        private SettingInfo settingInfo;
        public SettingVerbs(SettingInfo settingInfo)
        {
            this.settingInfo = settingInfo;
        }
        public IComponent Component => this.settingInfo;

        public IContainer Container => null;

        public bool DesignMode => false;

        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Dictionary<string, MethodInfo> verbMethods = new Dictionary<string, MethodInfo>();

        public DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection verbs = new DesignerVerbCollection();
                this.verbMethods.Clear();

                Type t = this.settingInfo.GetType();
                foreach (MethodInfo method in t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    object[] attrs = method.GetCustomAttributes(typeof(BrowsableAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        if (attrs.Cast<BrowsableAttribute>().All(b => b.Browsable))
                        {
                            this.verbMethods.Add(method.Name, method);
                            verbs.Add(new DesignerVerb(method.Name, new EventHandler(this.VerbEvent)));
                        }
                    }
                }

                return verbs;
            }
        }

        protected void VerbEvent(object sender, EventArgs e)
        {
            DesignerVerb verb = sender as DesignerVerb;
            MethodInfo method;
            if (this.verbMethods.TryGetValue(verb.Text, out method))
            {
                method.Invoke(this.settingInfo, null);
            }
        }



        public void AddCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void AddVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            return (serviceType == typeof(IMenuCommandService)) ? this : null;
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public void RemoveCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
            throw new NotImplementedException();
        }

    }

}
