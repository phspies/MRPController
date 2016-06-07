using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace wadltocsharp
{
    public class WadlProcess
    {
        public static string _file_string = "";

        public WadlProcess(String _file)
        {
            var wadl_object = new application();
            String xml = File.ReadAllText(_file);
            using (TextReader reader = new StringReader(xml))
            {
                wadl_object = (application)new XmlSerializer(typeof(application)).Deserialize(reader);
            }
            foreach (var _resource in wadl_object.resources.resource)
            {
                StreamWriter _file_object = new StreamWriter(String.Format("Classes/{0}.cs", UppercaseFirst(_resource.path.Replace("/",""))));
                _file_object.WriteLine("using MRMPService.RP4VM;\n");
                _file_object.WriteLine("using MRMPService.RP4VMAPI;\n");
                _file_object.WriteLine("using System.Collections.Generic;\n");

                _file_object.WriteLine("\n");

                _file_object.WriteLine("namespace MRMPService.RP4VM\n{\n");
                _file_object.WriteLine("public class " + UppercaseFirst(_resource.path.Replace("/", "") + " : Core\n{\n"));
                _file_object.WriteLine("public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }\n");
                ProcessNode(_resource, @"/" + _resource.path, _file_object);
                _file_object.WriteLine("\n}\n}");
                _file_object.Flush();
                _file_object.Close();
            }
        }
        public static void ProcessNode(Resource _resource, string _path, StreamWriter _file_object, List<Param> _parent_params = null)
        {
            List<Param> _child_params_ = null;
            foreach (var _child_resource in _resource.Items)
            {
                if (_child_resource is Param)
                {
                    if (_child_params_ == null)
                    {
                        _child_params_ = new List<Param>();
                    }
                    _child_params_.Add((Param)_child_resource);
                }
                if (_child_resource is Resource)
                {
                    ProcessNode((Resource)_child_resource, _path + @"/" + ((Resource)_child_resource).path, _file_object, _child_params_);
                }
            }
            string _function = "";
            List<Param> _child_params = new List<Param>();
            if (_parent_params != null)
            {
                _child_params.AddRange(_parent_params);
            }
            List<Method> _child_method = new List<Method>();
            List<Resource> _child_resources = new List<Resource>();
            foreach (var _child_item in _resource.Items)
            {
                if (_child_item is Method)
                {
                    _child_method.Add((Method)_child_item);
                }
                if (_child_item is Param)
                {
                    _child_params.Add((Param)_child_item);
                }
                if (_child_item is Resource)
                {
                    _child_resources.Add((Resource)_child_item);
                }
            }

            foreach (var _method in _child_method)
            {
                List<Param> _url_params = new List<Param>();
                if (_method.request != null)
                {
                    if (_method.request.param.Count() > 0)
                    {
                        _url_params.AddRange(_method.request.param);
                    }
                }
                _function = String.Format("public {0} {1}_Method(", (_method.response.representation.element == null ? "void" : UppercaseFirst(_method.response.representation.element)), _method.id);
                int _count = 0;
                foreach (var _param in _child_params)
                {
                    if (_count > 0)
                    {
                        _function += ",";
                    }
                    _function += String.Format("{0} {1}", _param.type.Replace("xs:", ""), _param.name);
                    _count += 1;
                }
                String _submit_object = null;
                if (_method.request != null)
                {
                    if (_method.request.representation != null)
                    {
                        _submit_object = _method.request.representation.element + "_object";
                        if (_count > 0)
                        {
                            _function += ",";
                        }
                        _function += String.Format("{0} {1}", _method.request.representation.element, _submit_object);
                        _count += 1;
                    }
                }
                foreach (var _param in _url_params)
                {
                    if (_count > 0)
                    {
                        _function += ",";
                    }
                    if (_param.type.Contains("long") || _param.type.Contains("int"))
                    {
                        _function += String.Format("{0}? {1}=null", _param.type.Replace("xs:", ""), _param.name);
                    }
                    else if (_param.type.Contains("boolean"))
                    {
                        _function += String.Format("bool? {0}=null", _param.name);
                    }
                    else
                    {
                        _function += String.Format("{0} {1}=null", _param.type.Replace("xs:", ""), _param.name);
                    }

                    _count += 1;
                }
                _function += ")\n{\n";
                String _tmp = "\tendpoint = " + '"' + "{0}" + '"' + ";\n";
                _function += String.Format(_tmp, _path);
                foreach (var _param in _child_params)
                {
                    _function += "\tendpoint.Replace(" + '"' + "{" + _param.name + "}" + '"' + ","  + _param.name + ".ToString());\n";

                }
                if (_url_params.Count > 0)
                {
                    foreach (var _url_param in _url_params)
                    {
                        _function += "if (" + _url_param.name + " != null) { url_params.Add(new KeyValuePair<string, string>(" + '"' + _url_param.name + '"' + ", " + _url_param.name + ".ToString()));}\n";
                    }
                    _function += "\n";
                }
                String _tmp2 = "\tmediatype=" + '"' + "{0}" + '"' + ";\n";
                _function += String.Format(_tmp2, _method.response.representation.mediaType);
                if (_submit_object != null)
                {
                    if (_method.response.representation.element != null)
                    {
                        _function += String.Format("\treturn {0}<{1}>({2});\n", _method.name.ToLower(), UppercaseFirst(_method.response.representation.element), _submit_object);
                    }
                    else
                    {
                        _function += String.Format("\t{0}({2});\n", _method.name.ToLower(), UppercaseFirst(_method.response.representation.element), _submit_object);
                    }
                }
                else

                {
                    if (_method.response.representation.element == null)
                    {
                        _function += String.Format("\t{0}({1});\n", _method.name.ToLower(), _submit_object);
                    }
                    else
                    {
                        _function += String.Format("\treturn {0}<{1}>({2});\n", _method.name.ToLower(), UppercaseFirst(_method.response.representation.element), _submit_object);
                    }
                }
                _function += "}\n\n";

            }
            _file_object.WriteLine(_function);
        }
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}