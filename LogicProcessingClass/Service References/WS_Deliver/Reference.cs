﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34011
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LogicProcessingClass.WS_Deliver {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WS_Deliver.DeliverSoap")]
    public interface DeliverSoap {
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 your_name 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Test", ReplyAction="*")]
        LogicProcessingClass.WS_Deliver.TestResponse Test(LogicProcessingClass.WS_Deliver.TestRequest request);
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 xml 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DeliverXml", ReplyAction="*")]
        LogicProcessingClass.WS_Deliver.DeliverXmlResponse DeliverXml(LogicProcessingClass.WS_Deliver.DeliverXmlRequest request);
        
        // CODEGEN: 命名空间 http://tempuri.org/ 的元素名称 sender_code 以后生成的消息协定未标记为 nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DeliverAffix", ReplyAction="*")]
        LogicProcessingClass.WS_Deliver.DeliverAffixResponse DeliverAffix(LogicProcessingClass.WS_Deliver.DeliverAffixRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class TestRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Test", Namespace="http://tempuri.org/", Order=0)]
        public LogicProcessingClass.WS_Deliver.TestRequestBody Body;
        
        public TestRequest() {
        }
        
        public TestRequest(LogicProcessingClass.WS_Deliver.TestRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class TestRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string your_name;
        
        public TestRequestBody() {
        }
        
        public TestRequestBody(string your_name) {
            this.your_name = your_name;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class TestResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="TestResponse", Namespace="http://tempuri.org/", Order=0)]
        public LogicProcessingClass.WS_Deliver.TestResponseBody Body;
        
        public TestResponse() {
        }
        
        public TestResponse(LogicProcessingClass.WS_Deliver.TestResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class TestResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string TestResult;
        
        public TestResponseBody() {
        }
        
        public TestResponseBody(string TestResult) {
            this.TestResult = TestResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DeliverXmlRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="DeliverXml", Namespace="http://tempuri.org/", Order=0)]
        public LogicProcessingClass.WS_Deliver.DeliverXmlRequestBody Body;
        
        public DeliverXmlRequest() {
        }
        
        public DeliverXmlRequest(LogicProcessingClass.WS_Deliver.DeliverXmlRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class DeliverXmlRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public byte[] xml;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string filename;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string sender_code;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string receiver_code;
        
        public DeliverXmlRequestBody() {
        }
        
        public DeliverXmlRequestBody(byte[] xml, string filename, string sender_code, string receiver_code) {
            this.xml = xml;
            this.filename = filename;
            this.sender_code = sender_code;
            this.receiver_code = receiver_code;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DeliverXmlResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="DeliverXmlResponse", Namespace="http://tempuri.org/", Order=0)]
        public LogicProcessingClass.WS_Deliver.DeliverXmlResponseBody Body;
        
        public DeliverXmlResponse() {
        }
        
        public DeliverXmlResponse(LogicProcessingClass.WS_Deliver.DeliverXmlResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class DeliverXmlResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string DeliverXmlResult;
        
        public DeliverXmlResponseBody() {
        }
        
        public DeliverXmlResponseBody(string DeliverXmlResult) {
            this.DeliverXmlResult = DeliverXmlResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DeliverAffixRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="DeliverAffix", Namespace="http://tempuri.org/", Order=0)]
        public LogicProcessingClass.WS_Deliver.DeliverAffixRequestBody Body;
        
        public DeliverAffixRequest() {
        }
        
        public DeliverAffixRequest(LogicProcessingClass.WS_Deliver.DeliverAffixRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class DeliverAffixRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int position;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string sender_code;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string filename;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public byte[] affix;
        
        public DeliverAffixRequestBody() {
        }
        
        public DeliverAffixRequestBody(int position, string sender_code, string filename, byte[] affix) {
            this.position = position;
            this.sender_code = sender_code;
            this.filename = filename;
            this.affix = affix;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DeliverAffixResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="DeliverAffixResponse", Namespace="http://tempuri.org/", Order=0)]
        public LogicProcessingClass.WS_Deliver.DeliverAffixResponseBody Body;
        
        public DeliverAffixResponse() {
        }
        
        public DeliverAffixResponse(LogicProcessingClass.WS_Deliver.DeliverAffixResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class DeliverAffixResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string DeliverAffixResult;
        
        public DeliverAffixResponseBody() {
        }
        
        public DeliverAffixResponseBody(string DeliverAffixResult) {
            this.DeliverAffixResult = DeliverAffixResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface DeliverSoapChannel : LogicProcessingClass.WS_Deliver.DeliverSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DeliverSoapClient : System.ServiceModel.ClientBase<LogicProcessingClass.WS_Deliver.DeliverSoap>, LogicProcessingClass.WS_Deliver.DeliverSoap {
        
        public DeliverSoapClient() {
        }
        
        public DeliverSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DeliverSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DeliverSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DeliverSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LogicProcessingClass.WS_Deliver.TestResponse LogicProcessingClass.WS_Deliver.DeliverSoap.Test(LogicProcessingClass.WS_Deliver.TestRequest request) {
            return base.Channel.Test(request);
        }
        
        public string Test(string your_name) {
            LogicProcessingClass.WS_Deliver.TestRequest inValue = new LogicProcessingClass.WS_Deliver.TestRequest();
            inValue.Body = new LogicProcessingClass.WS_Deliver.TestRequestBody();
            inValue.Body.your_name = your_name;
            LogicProcessingClass.WS_Deliver.TestResponse retVal = ((LogicProcessingClass.WS_Deliver.DeliverSoap)(this)).Test(inValue);
            return retVal.Body.TestResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LogicProcessingClass.WS_Deliver.DeliverXmlResponse LogicProcessingClass.WS_Deliver.DeliverSoap.DeliverXml(LogicProcessingClass.WS_Deliver.DeliverXmlRequest request) {
            return base.Channel.DeliverXml(request);
        }
        
        public string DeliverXml(byte[] xml, string filename, string sender_code, string receiver_code) {
            LogicProcessingClass.WS_Deliver.DeliverXmlRequest inValue = new LogicProcessingClass.WS_Deliver.DeliverXmlRequest();
            inValue.Body = new LogicProcessingClass.WS_Deliver.DeliverXmlRequestBody();
            inValue.Body.xml = xml;
            inValue.Body.filename = filename;
            inValue.Body.sender_code = sender_code;
            inValue.Body.receiver_code = receiver_code;
            LogicProcessingClass.WS_Deliver.DeliverXmlResponse retVal = ((LogicProcessingClass.WS_Deliver.DeliverSoap)(this)).DeliverXml(inValue);
            return retVal.Body.DeliverXmlResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LogicProcessingClass.WS_Deliver.DeliverAffixResponse LogicProcessingClass.WS_Deliver.DeliverSoap.DeliverAffix(LogicProcessingClass.WS_Deliver.DeliverAffixRequest request) {
            return base.Channel.DeliverAffix(request);
        }
        
        public string DeliverAffix(int position, string sender_code, string filename, byte[] affix) {
            LogicProcessingClass.WS_Deliver.DeliverAffixRequest inValue = new LogicProcessingClass.WS_Deliver.DeliverAffixRequest();
            inValue.Body = new LogicProcessingClass.WS_Deliver.DeliverAffixRequestBody();
            inValue.Body.position = position;
            inValue.Body.sender_code = sender_code;
            inValue.Body.filename = filename;
            inValue.Body.affix = affix;
            LogicProcessingClass.WS_Deliver.DeliverAffixResponse retVal = ((LogicProcessingClass.WS_Deliver.DeliverSoap)(this)).DeliverAffix(inValue);
            return retVal.Body.DeliverAffixResult;
        }
    }
}
