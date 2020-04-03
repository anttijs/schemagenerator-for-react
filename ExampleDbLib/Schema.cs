using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace ExampleDbLib
{
    [NotMapped]
    public class EnumDesc
    {
        public int value { get; set; }
        public string text { get; set; }
    }
    public interface IProps
    {
        List<EnumDesc> GetPropEnums(object obj, ExampleDbContext context);
    }
    [NotMapped]
    public class Prop
    {
        public bool Hidden { get; set; } = false;
        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string InputType { get; set; }
        public bool Readonly { get; set; } = false;
        public bool Required { get; set; } = true;
        public object Min { get; set; }
        public object Max { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string Pattern { get; set; }
        public List<EnumDesc> PropEnums { get; set; }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class HiddenAttribute : Attribute
    {

        public HiddenAttribute()
        {
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionsSourceAttribute : Attribute
    {
        private string name;


        public OptionsSourceAttribute(string ClassName)
        {
            this.name = ClassName;
        }
        public virtual string ClassName
        {
            get { return name; }
        }
    }
    [NotMapped]
    public class ReplyData
    {
        public Object data { get; set; }
        public Schema schema { get; set; }
    }
    [NotMapped]
    public class Schema
    {
        public string Display { get; set; }
        public string ClassName { get; set; }
        public string KeyName { get; set; }
        public List<Prop> Props { get; set; } = new List<Prop>();
        public Schema(Object obj, ExampleDbContext context)
        {

            this.ClassName = obj.GetType().Name;
            this.Display = this.ClassName;

            System.Reflection.MemberInfo info = obj.GetType();
            object[] attributes = info.GetCustomAttributes(true);

            foreach (object attr in attributes)
            {
                if (attr is DisplayAttribute x)
                {
                    this.Display = x.Name;
                }
            }

            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                Prop pr = new Prop();
                bool add = true;

                if (prop.CanWrite == false)
                    pr.Readonly = true;
                pr.Name = prop.Name;
                pr.Title = prop.Name;
                if (pr.Name == $"{this.ClassName}Id" || pr.Name == "Id")
                    this.KeyName = pr.Name;

                pr.Type = GetType(prop.PropertyType);
                pr.InputType = pr.Type; //initial state, DataTypeAttribute may change this
                pr.Required = GetRequired(prop.PropertyType);
                pr.Max = GetRangeMax(prop.PropertyType);
                pr.Min = GetRangeMin(prop.PropertyType);
                pr.PropEnums = GetDescriptions(prop.PropertyType);

                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is DisplayAttribute da)
                    {
                        pr.Title = da.Name;
                    }
                    else if (attr is DataTypeAttribute dta)
                    {
                        pr.InputType = ToInputType(dta.DataType);
                    }
                    else if (attr is HiddenAttribute h)
                    {
                        pr.Hidden = true;
                    }
                    else if (attr is ReadOnlyAttribute r)
                    {
                        pr.Readonly = r.IsReadOnly;
                    }
                    else if (attr is JsonIgnoreAttribute j)
                    {
                        add = false;
                    }
                    else if (attr is OptionsSourceAttribute o)
                    {
                        Type magicType = Type.GetType(o.ClassName);
                        ConstructorInfo magicConstructor = magicType.GetConstructor(Type.EmptyTypes);
                        object magicClassObject = magicConstructor.Invoke(new object[] { });
                        MethodInfo magicMethod = magicType.GetMethod("GetPropEnums");
                        object magicValue = magicMethod.Invoke(magicClassObject, new object[] { obj, context });
                        pr.PropEnums = magicValue as List<EnumDesc>;
                        pr.Type = "enum";
                    }
                    else if (attr is RequiredAttribute re)
                    {
                        pr.Required = true;
                    }
                    else if (attr is RangeAttribute ra)
                    {
                        pr.Min = ra.Minimum;
                        pr.Max = ra.Maximum;
                    }
                    else if (attr is StringLengthAttribute sl)
                    {
                        pr.MaxLength = sl.MaximumLength;
                        pr.MinLength = sl.MinimumLength;
                    }
                    else if (attr is RegularExpressionAttribute reg)
                    {
                        pr.Pattern = reg.Pattern;
                    }
                }
                if (add == true)
                    this.Props.Add(pr);
            }

        }
        private static string ToInputType(DataType t)
        {
            string retval = "text";
            switch (t)
            {
                case DataType.Custom:
                    break;
                case DataType.DateTime:
                    retval = "datetime-local";
                    break;
                case DataType.Date:
                    retval = "date";
                    break;
                case DataType.Time:
                    retval = "time";
                    break;
                case DataType.Duration:
                    break;
                case DataType.PhoneNumber:
                    retval = "tel";
                    break;
                case DataType.Currency:
                    break;
                case DataType.Text:
                    break;
                case DataType.Html:
                    break;
                case DataType.MultilineText:
                    break;
                case DataType.EmailAddress:
                    retval = "email";
                    break;
                case DataType.Password:
                    retval = "password";
                    break;
                case DataType.Url:
                    break;
                case DataType.ImageUrl:
                    break;
                case DataType.CreditCard:
                    break;
                case DataType.PostalCode:
                    break;
                case DataType.Upload:
                    break;
                default:
                    break;
            }
            return retval;
        }
        private static string GetType(Type t)
        {
            if (t == typeof(bool))
                return "bool";
            
            if (IsEnum(t))
                return "enum";

            if (IsNumericType(t))
                return "number";
            
            return "text";
                
        }
        private static bool GetRequired(Type t)
        {
            return !(t.Name == "String" || Nullable.GetUnderlyingType(t) != null);
        }
        private static object GetRangeMax(Type t)
        {
            if (IsNumericType(t) == false)
                return null;

            var underlyingType = Nullable.GetUnderlyingType(t);
            if (underlyingType != null)
                t = underlyingType;

            FieldInfo fi = t.GetField("MaxValue");
            if (fi != null && fi.IsLiteral && !fi.IsInitOnly)
                return fi.GetRawConstantValue();

            return null;

        }

        private static object GetRangeMin(Type t)
        {
            if (IsNumericType(t) == false)
                return null;

            var underlyingType = Nullable.GetUnderlyingType(t);
            if (underlyingType != null)
                t = underlyingType;

            FieldInfo fi = t.GetField("MinValue");
            if (fi != null && fi.IsLiteral && !fi.IsInitOnly)
                return fi.GetRawConstantValue();

            return null;

        }
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };
        
        private static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }
        private static bool IsEnum(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return type.IsEnum || (underlyingType != null && underlyingType.IsEnum);
        }
        public static List<EnumDesc> GetDescriptions(Type t)
        {
            var underlyingType = Nullable.GetUnderlyingType(t);
            if (underlyingType != null)
                t = underlyingType;

            if (t.IsEnum == false)
                return null;

            List<EnumDesc> dic = new List<EnumDesc>();
            Array values = Enum.GetValues(t);
            foreach (int val in values)
            {

                var memInfo = t.GetMember(t.GetEnumName(val));
                var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptionAttributes.Length > 0)
                {
                    // we're only getting the first description we find
                    // others will be ignored
                    dic.Add(new EnumDesc { value = val, text = ((DescriptionAttribute)descriptionAttributes[0]).Description });
                }
            }

            return dic;
        }
    }
}
