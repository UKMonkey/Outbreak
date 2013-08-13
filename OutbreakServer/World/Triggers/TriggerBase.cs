using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.World.Triggers;

namespace Outbreak.Server.World.Triggers
{
    public abstract class TriggerBase : ITrigger
    {
        public TriggerActivated OnActivated { get; set; }

        private TriggerKey _uniqueKey;
        public virtual TriggerKey UniqueKey { get { return _uniqueKey; }  }

        private Vector3 _location;
        public virtual Vector3 Location { get { return _location; } }

        private readonly string _name;
        public virtual string Name { get { return _name; } }

        public virtual TriggerActivationMethod ActivationMethod
            { get { throw new NotImplementedException(); } }

        public IDictionary<string, string> Properties { get; private set; }

        public virtual bool SendToClient 
            { get { throw new NotImplementedException(); } }

        protected IEngine Engine { get; private set; }


        public virtual void SetProperties(TriggerKey key, Vector3 location, IEnumerable<KeyValuePair<string, string>> properties)
        {
            _uniqueKey = key;
            _location = location;
            Properties.Clear();
            foreach (var item in properties)
            {
                Properties.Add(item.Key, item.Value);
            }
        }

        public IDictionary<string, string> GetProperties()
        {
            return Properties;
        }

        protected TriggerBase(IEngine engine)
        {
            _name = GetName();

            Engine = engine;
            Properties = new Dictionary<string, string>();
            Debug.Assert(_name != "");
        }

        protected TriggerBase(IEngine engine, TriggerKey uniqueKey, Vector3 location)
        {
            _uniqueKey = uniqueKey;
            _location = location;
            _name = GetName();

            Engine = engine;
            Properties = new Dictionary<string, string>();
            Debug.Assert(_name != "");
        }

        protected string GetName()
        {
            var attrs = Attribute.GetCustomAttributes(GetType());
            foreach (var reta in attrs.OfType<TriggerTypeAttribute>().Select(attr => attr))
            {
                return reta.TypeName;
            }
            return "";
        }

        public abstract void Update();
    }
}
