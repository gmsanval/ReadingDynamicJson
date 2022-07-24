using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicJsonReading.DynamicDataLogic
{
    public class DynamicJsonRead
    {
        public static dynamic Read(string path)
        {
            using (StreamReader file = new StreamReader(path))
            {
                try
                {
                    string json = file.ReadToEnd();

                    var serializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    var responseData = JsonConvert.DeserializeObject<object>(json);
                    var type = responseData.GetType();

                    if (type.Name == "JArray")
                    {
                        JArray jarray = JArray.Parse(json) as JArray;
                        foreach (var prop in jarray.GetType().GetProperties())
                        {
                            Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(jarray, null));
                        }
                    }
                    if (type.Name == "JObject")
                    {
                        JObject jobject = JObject.Parse(json) as JObject;
                        foreach (var prop in jobject.Properties())
                        {
                            if (prop.Value.Type == JTokenType.Property)
                            {
                                DealWithProperty(prop);
                            }
                            if (prop.Value.Type == JTokenType.Object)
                            {
                                JObject jobject1 = JObject.Parse(prop.Value.ToString()) as JObject;
                                DealWithJObject(jobject1);
                            }
                            if (prop.Value.Type == JTokenType.Array)
                            {
                                JArray jarray = JArray.Parse(json) as JArray;
                                foreach (var prop2 in jarray)
                                {
                                    JObject jobject1 = JObject.Parse(prop2.ToString()) as JObject;
                                    foreach (var prop1 in jobject1.Properties())
                                    {
                                        Console.WriteLine("{0}:{1}", prop1.Name, prop1.Value);
                                    }
                                }
                            }
                            Console.WriteLine("{0}:{1}", prop.Name, prop.Value);
                        }
                    }
                    return JsonConvert.DeserializeObject<dynamic>(json, serializerSettings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem reading file");
                    return null;
                }
            }
        }


        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }


        public static void DealWithJArray(JArray jArray)
        {
            JArray jarray = JArray.Parse(jArray.ToString()) as JArray;
            foreach (var jobj in jarray)
            {
                JObject jobject = JObject.Parse(jobj.ToString()) as JObject;
                DealWithJObject(jobject);
            }
        }

        public static void DealWithProperty(JProperty jProperty)
        {
            if (jProperty.Value.Type == JTokenType.String)
            {
                Console.WriteLine("{0}:{1}", jProperty.Name, jProperty.Value);
            }
            if (jProperty.Value.Type == JTokenType.Integer)
            {
                Console.WriteLine("{0}:{1}", jProperty.Name, jProperty.Value);
            }
            else if (jProperty.Value.Type == JTokenType.Object)
            {
                JObject jobject = JObject.Parse(jProperty.Value.ToString()) as JObject;
                foreach (var prop in jobject.Properties())
                {
                    if (prop.Value.Type == JTokenType.Object)
                    {
                        DealWithJObject(jobject);
                    }
                    else if (prop.Value.Type == JTokenType.Array)
                    {
                        JArray jarray = JArray.Parse(prop.Value.ToString()) as JArray;
                        DealWithJArray(jarray);
                    }
                    else if (prop.Value.Type == JTokenType.String)
                    {
                        Console.WriteLine("{0}:{1}", prop.Name, prop.Value);
                    }
                    else if (prop.Value.Type == JTokenType.Integer)
                    {
                        Console.WriteLine("{0}:{1}", prop.Name, prop.Value);
                    }

                }
            }
            else if (jProperty.Value.Type == JTokenType.Array)
            {
                JArray jarray = JArray.Parse(jProperty.Value.ToString()) as JArray;
                DealWithJArray(jarray);
            }
        }

        public static void DealWithJObject(JObject keyValues)
        {

            foreach (var prop in keyValues.Properties())
            {
                DealWithProperty(prop);
            }

        }
    }


}
