﻿using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using XmlRpc_Wrapper;

namespace rosmaster
{
    public class Master_API
    {
        public readonly int STATUS = 0;
        public readonly int MSG = 1;
        public readonly int VAL = 2;

        public class ROSMasterHandler
        {
            #region Master Implementation

            private String uri;
            private bool done = false;

            RegistrationManager reg_manager;
            Registrations publishers;
            Registrations subscribers;
            Registrations services;
            Registrations param_subscribers;

            //public Dictionary<String, String> topic_types;

            Dictionary<String, String> topic_types; // {topicName : type}

            ParamDictionary param_server;


            public void mloginfo() { }
            public void mlogwarn() { }
            public void apivalidate() { }
            public List<String> publisher_update_task( String api, String topic, List<string> pub_uris) 
            {
                XmlRpcValue l = new XmlRpcValue();
                l.Set(0, api);
                l.Set(1, "");
                //XmlRpcValue ll = new XmlRpcValue();
                //l.Set(0, ll);
                for(int i = 0; i < pub_uris.Count; i++)
                {
                    XmlRpcValue ll = new XmlRpcValue(pub_uris[i]);
                    l.Set(i + 1, ll);
                }


                XmlRpcValue args = new XmlRpcValue();
                args.Set(0, "master");
                args.Set(1, topic);
                args.Set(2, l);
                       XmlRpcValue result = new XmlRpcValue(new XmlRpcValue(), new XmlRpcValue(), new XmlRpcValue(new XmlRpcValue())),
                        payload = new XmlRpcValue();

                 Ros_CSharp.master.host = api.Replace("http://","").Replace("/","").Split(':')[0];
                 Ros_CSharp.master.port =  int.Parse( api.Replace("http://", "").Replace("/", "").Split(':')[1]);
                 Ros_CSharp.master.execute("publisherUpdate", args, result, payload, false );
                
                return new List<string>(new []{"http://ERIC:1337"});
            }

            public void service_update_task(String api, String service, String uri) 
            {
                XmlRpcValue args = new XmlRpcValue();
                args.Set(0, "master");
                args.Set(1, service);
                args.Set(2, uri);
                XmlRpcValue result = new XmlRpcValue(new XmlRpcValue(), new XmlRpcValue(), new XmlRpcValue(new XmlRpcValue())),
                 payload = new XmlRpcValue();

                Ros_CSharp.master.host = api.Replace("http://", "").Replace("/", "").Split(':')[0];
                Ros_CSharp.master.port = int.Parse(api.Replace("http://", "").Replace("/", "").Split(':')[1]);
                Ros_CSharp.master.execute("publisherUpdate", args, result, payload, false);
            }

            public ROSMasterHandler()
            {
                reg_manager = new RegistrationManager();

                 publishers = reg_manager.publishers;
                 subscribers =  reg_manager.subscribers;
                 services = reg_manager.services;
                 param_subscribers = reg_manager.param_subscribers;

                 topic_types = new Dictionary<String, String>();
                 param_server = new rosmaster.ParamDictionary(reg_manager);

            }

            public void _shutdown(String reason="")
            {
                //TODO:THREADING
                done = true;
            }
            public void _ready(String _uri)
            {
                uri = _uri;
            }
            public Boolean _ok()
            {
                return !done;
            }
            public void shutdown(String caller_id, String msg = "")
            {

            }
            public String getUri(String caller_id)
            {
                return uri;
            }
            public int getPid(String caller_id)
            {
                return Process.GetCurrentProcess().Id;
            }
            #endregion

            #region PARAMETER SERVER ROUTINES


            public int deleteParam(String caller_id, String key) 
            {
                try
                {
                    key = Names.resolve_name(key, caller_id);
                    param_server.delete_param(key, _notify_param_subscribers);
                    return 1;
                }
                catch (KeyNotFoundException e) { return -1; }
            }
            public void setParam(String caller_id, String key, XmlRpcValue value) 
            {
                key = Names.resolve_name(key,caller_id);
                param_server.set_param(key, value, _notify_param_subscribers);
            }

            /// <summary>
            /// Returns Param if it exists, null if it doesn't
            /// </summary>
            /// <param name="caller_id"></param>
            /// <param name="key"></param>
            /// <returns>Dictionary</Dictionary></returns>
            public XmlRpcValue getParam(String caller_id, String key)
            {
                try
                {
                    key = Names.resolve_name(key, caller_id);
                    return param_server.get_param(key);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            public String searchParam(String caller_id, String key) 
            {
                String search_key = param_server.search_param(caller_id,key);
                return search_key;
            }
            public XmlRpcValue subscribeParam(String caller_id, String caller_api, String key) 
            {
                key = Names.resolve_name(key,caller_id);

                try
                {
                    return param_server.subscribe_param(key, caller_id, caller_api);
                }catch(Exception e)
                {
                    return null;
                }
            }

            public ReturnStruct unsubscribeParam(String caller_id, String caller_api, String key) 
            {
                key = Names.resolve_name(key, caller_id);
                return param_server.unsubscribe_param(key, caller_id, caller_api);
            }

            public bool hasParam(String caller_id, String key) 
            {
                key = Names.resolve_name(key, caller_id);
                return param_server.has_param(key);
            }

            public List<String> getParamNames(String caller_id) 
            {
                return param_server.get_param_names();
            }

            #endregion




            #region NOTIFICATION ROUTINES

            public void _notify(Registrations r, Func< String, String, List<String>, List<String>> task, String key, List<String> value, List<String> node_apis) 
            {
                foreach (String node_api in node_apis)
                {
                   // if (node_api != null && node_uris.Count > 0)
                    //{
                    task(node_api, key, value);
                    //}
                }
            }
            public int _notify_param_subscribers(Dictionary<String, Tuple<String, XmlRpc_Wrapper.XmlRpcValue>> updates) 
            { 
                return 1; 
            }

            public void _param_update_task(String caller_id, String caller_api, String param_key, XmlRpc_Wrapper.XmlRpcValue param_value) 
            { 

            }

            public void _notify_topic_subscribers(String topic, List<String> pub_uris, List<String> sub_uris ) 
            {
                _notify(subscribers, publisher_update_task, topic, pub_uris, sub_uris);
            }

            public void _notify_service_update(String service, String service_api) 
            {
                
            }

            #endregion


            #region SERVICE PROVIDER
            public ReturnStruct registerService(String caller_id, String service, String service_api, String caller_api) 
            {
                reg_manager.register_service(service, caller_id, caller_api, service_api);
                return new ReturnStruct(1, "Registered [" + caller_id + "] as provider of [" + service +"]", new XmlRpcValue(1));
            }

            public ReturnStruct lookupService(String caller_id, String service) 
            {
                String service_url = services.get_service_api(service);

                if(service_url != null && service_url.Length > 0)
                    return new ReturnStruct(1, "rosrpc URI: [" + service_url + "]", new XmlRpcValue(service_url)); 
                else
                    return new ReturnStruct(-1, "No provider");
            }

            public ReturnStruct unregisterService(String caller_id, String service, String service_api) 
            {
                return reg_manager.unregister_service(service, caller_id, service_api);
                //return new ReturnStruct(1, "Registered [" + caller_id + "] as provider of [" + service + "]", new XmlRpcValue(1));
            }

            #endregion



            #region PUBLISH/SUBSCRIBE

            public ReturnStruct registerSubscriber(String caller_id, String topic, String topic_type, String caller_api) 
            {
                reg_manager.register_subscriber(topic, caller_id, caller_api);

                if (!topic_types.ContainsValue(topic_type))
                    topic_types.Add(topic, topic_type);
                List<String> puburis = publishers.get_apis(topic);

                ReturnStruct rtn = new ReturnStruct();
                rtn.statusMessage = String.Format("Subscribed to [{0}] ", topic);
                rtn.statusCode = 1;
                rtn.value = new XmlRpcValue();
                rtn.value.Set(0, new XmlRpcValue());
                for (int i = 0; i < puburis.Count(); i++)
                {
                    XmlRpcValue tmp = new XmlRpcValue(puburis[i]);
                    rtn.value.Set(i, tmp);
                }
                return rtn;
            }

            public int unregisterSubscriber(String caller_id, String topic, String caller_api) 
            {
                reg_manager.unregister_subscriber(topic, caller_id, caller_api);
                return 1;
            }

            /// <summary>
            /// Register a publisher
            /// </summary>
            /// <param name="caller_id"></param>
            /// <param name="topic"></param>
            /// <param name="topic_type"></param>
            /// <param name="caller_api"></param>
            /// <returns></returns>
            public ReturnStruct registerPublisher(String caller_id, String topic, String topic_type, String caller_api) 
            {
                reg_manager.register_publisher(topic, caller_id, caller_api);
                if (!topic_types.ContainsValue(topic_type))
                    topic_types.Add(topic, topic_type);
                List<String> puburis = publishers.get_apis(topic);
                List<String> sub_uris = subscribers.get_apis(topic);
                _notify_topic_subscribers(topic, puburis, sub_uris);

                ReturnStruct rtn = new ReturnStruct();
                rtn.statusMessage = String.Format("Registered [{0}] as publisher of [{1}]", caller_id, topic);
                rtn.statusCode = 1;
                rtn.value = new XmlRpcValue();
                rtn.value.Set(0, new XmlRpcValue());
                for (int i = 0; i < sub_uris.Count(); i++)
                {
                    XmlRpcValue tmp = new XmlRpcValue(sub_uris[0]);
                    rtn.value.Set(i, tmp);
                }
                return rtn;
            }
            public int unregisterPublisher(String caller_id, String topic, String caller_api) 
            {
                reg_manager.unregister_publisher(topic, caller_id, caller_api);
                return 1;
            }

            #endregion


            #region GRAPH STATE API
            public String lookupNode(String caller_id, String node_name) 
            {
                NodeRef node = reg_manager.get_node(caller_id);
                if (node == null)
                    return "";
                return node.api;


            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="caller_id">Not used?Wtf?</param>
            /// <param name="subgraph">Optional String, only returns topics that start with that name</param>
            /// <returns></returns>
            public List<List<String>> getPublishedTopics(String caller_id, String subgraph) 
            {
                if (subgraph != "" && !subgraph.EndsWith("/"))
                    subgraph = subgraph + "/";
                Dictionary<String, List<List<String>>> e = new Dictionary<String, List<List<String>>>(publishers.map);
                List<List<String>> rtn = new List<List<String>>();

                foreach (KeyValuePair<String,List<List<String>>> pair in e)
                {
                    if (pair.Key.StartsWith(subgraph))
                        foreach (List<String> s in pair.Value)
                        {
                            List<String> value = new List<string>();
                            value.Add(pair.Key);
                            value.Add(topic_types[pair.Key]);
                            rtn.Add(value);
                        }
                }
                return rtn;
            }
            public Dictionary<String,String> getTopicTypes(String caller_id) 
            {
                return new Dictionary<String,String>(topic_types);
            }

            public List<List<List<String>>> getSystemState(String caller_id) 
            {
                List<List<List<String>>> rtn = new List<List<List<String>>>();
                rtn.Add(publishers.get_state());
                rtn.Add(subscribers.get_state());
                rtn.Add(services.get_state());
                return rtn;
            }

            #endregion
        }
    }
}
