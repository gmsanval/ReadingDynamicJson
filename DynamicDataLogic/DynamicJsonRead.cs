using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicJsonReading.DynamicDataLogic
{
    public class DynamicJsonRead
    {
        public static IDictionary<string,object>? dictionary = null;
        public static IDictionary<string, object>? newProperiesList = null;
        public static IDictionary<string, object> Read(string path)
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
                            //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(jarray, null));
                        }
                    }
                    if (type.Name == "JObject")
                    {
                        JObject jobject = JObject.Parse(json) as JObject;
                        
                        foreach (var prop in jobject.Properties())
                        {
                            if (prop.Value.Type == JTokenType.Object)
                            {
                                var obj = new JObject();
                                dynamic flexible = new ExpandoObject();
                                dictionary = (IDictionary<string, object>)flexible;
                                dictionary.Add(prop.Name, obj);
                                CreateJsonProperty(prop, true);
                            }
                            if (prop.Value.Type == JTokenType.String)
                            {
                                CreateJsonProperty(prop, false);
                            }
                            if (prop.Value.Type == JTokenType.Integer)
                            {
                                CreateJsonProperty(prop, false);
                            }
                            if (prop.Value.Type == JTokenType.Boolean)
                            {
                                CreateJsonProperty(prop, false);
                            }
                        }
                    }
                    return dictionary;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem reading file");
                    return null;
                }
            }
        }
        public static void CreateJsonProperty(JProperty property,bool isObjectProp)
        {
            if(isObjectProp == false && property.Value.Type == JTokenType.Property)
            {
                dictionary.Add(property.Name, property.Value);
            }
            if (isObjectProp == true && property.Value.Type == JTokenType.Object)
            {
                JObject jobject = JObject.Parse(property.Value.ToString()) as JObject;
                CreateJsonObjectProperties(jobject, true, property.Name);
            }


        }

        public static void CreateJsonObjectProperties(JObject joject, bool isObjectProp,string key)
        {
            dynamic flexible = new ExpandoObject();
            var newProperies = (IDictionary<string, object>)flexible;
            foreach (var prop in joject.Properties())
            {
                if(prop.Value.Type == JTokenType.Array)
                {
                    dictionary[key] = JsonConvert.SerializeObject(newProperies);
                    JArray jarray = JArray.Parse(prop.Value.ToString()) as JArray;
                    CreateJsonArryObjects(jarray, key, prop.Name);
                }
                else
                {
                    newProperies.Add(prop.Name, prop.Value);
                    dictionary[key] = JsonConvert.SerializeObject(newProperies);
                }
            }
        }
        public static void CreateJsonArryObjects(JArray jArray,string parentKey, string key)
        {
            dynamic flexible1 = new ExpandoObject();
            newProperiesList = (IDictionary<string, object>)flexible1;
            foreach (var jobj in jArray)
            {
                JObject jobject = JObject.Parse(jobj.ToString()) as JObject;
                CreateJsonArrayObjectProperties(jobject, parentKey, key);
            }
        }

        public static void CreateJsonArrayObjectProperties(JObject joject, string parentKey, string key)
        {
            dynamic flexible = new ExpandoObject();
            var newProperies = (IDictionary<string, object>)flexible;
            foreach (var prop in joject.Properties())
            {
                if (prop.Value.Type == JTokenType.Object)
                {
                    newProperiesList.Add(prop.Name, new JObject());
                    JObject jobject = JObject.Parse(prop.Value.ToString()) as JObject;
                    CreateJsonArryObjectProperties(jobject, key, prop.Name);
                }
                else
                {
                    newProperies.Add(prop.Name, prop.Value);
                    dictionary[key] = JsonConvert.SerializeObject(newProperies);
                }
            }
            
        }

        public static void CreateJsonArryObjectProperties(JObject joject, string parentKey, string key)
        {
            dynamic flexible = new ExpandoObject();
            var newProperies = (IDictionary<string, object>)flexible;
            foreach (var prop in joject.Properties())
            {
                if (prop.Value.Type == JTokenType.Object)
                {
                    JObject jobject = JObject.Parse(prop.Value.ToString()) as JObject;
                    CreateJsonArryObjectProperties(jobject, key, prop.Name);
                }
                else if (prop.Value.Type == JTokenType.String || prop.Value.Type == JTokenType.Integer)
                {
                    newProperies.Add(prop.Name, prop.Value);
                }
            }
            newProperiesList[key] = JsonConvert.SerializeObject(newProperies);
            dictionary[parentKey] = JsonConvert.SerializeObject(newProperiesList);
        }
    }
}
