namespace RuiSantos.Labs.GraphQL;

[AttributeUsage(AttributeTargets.Interface)]
internal class AdapterAttribute: Attribute { 
    public Type InstanceType { get; }

    public AdapterAttribute(Type instanceType)    
    {
        this.InstanceType = instanceType;
    } 
} 