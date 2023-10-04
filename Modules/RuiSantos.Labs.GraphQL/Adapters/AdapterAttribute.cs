namespace RuiSantos.Labs.GraphQL.Adapters;

[AttributeUsage(AttributeTargets.Interface)]
internal class AdapterAttribute: Attribute { 
    public Type InstanceType { get; }

    public AdapterAttribute(Type instanceType)    
    {
        InstanceType = instanceType;
    } 
} 