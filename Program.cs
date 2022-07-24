

using DynamicJsonReading.DynamicDataLogic;
using Newtonsoft.Json;

var rootPath = AppDomain.CurrentDomain.BaseDirectory;
rootPath = rootPath.Split("bin")[0];
var jsonRequestFile = Path.Combine(rootPath, "JsonFiles\\requests.json");
var jsonValueFile = Path.Combine(rootPath, "JsonFiles\\values.json");

var data = DynamicJsonRead.Read(jsonRequestFile);

Console.ReadLine();
