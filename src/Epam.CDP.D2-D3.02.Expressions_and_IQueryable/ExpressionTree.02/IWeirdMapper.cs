namespace ExpressionTree._02
{
    public interface IWeirdMapper
    {
        void CreateMap<TSource, TDestination>();
        TDestination Map<TSource, TDestination>(TSource source);
    }
}
