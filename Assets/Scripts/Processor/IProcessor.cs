
using UnityEngine;

public interface IProcessor<in TIn, out TOut>
{
    TOut Process(TIn input);
}

public delegate TOut ProcessorDelegate<in TIn, out TOut>(TIn input);

class Chain<TIn, TOut>
{
    readonly IProcessor<TIn, TOut> processor;
    
    Chain(IProcessor<TIn, TOut> processor) => this.processor = processor;

    public static Chain<TIn, TOut> Start(IProcessor<TIn, TOut> processor)
    {
        return new Chain<TIn, TOut>(processor);
    }

    public Chain<TIn, TNext> Then<TNext>(IProcessor<TOut, TNext> next)
    {
        var combined = new Combined<TIn, TOut, TNext>(processor, next);
        return new Chain<TIn, TNext>(combined);
    }
    
    public TOut Run(TIn input) => processor.Process(input);
    public ProcessorDelegate<TIn, TOut> Compile() => input => processor.Process(input);
}

public class Combined<A, B, C> : IProcessor<A, C>
{
    readonly IProcessor<A, B> processorA;
    readonly IProcessor<B, C> processorB;
    public Combined(IProcessor<A, B> processorA, IProcessor<B, C> processorB)
    {
        this.processorA = processorA;
        this.processorB = processorB;
    }
    public C Process(A input) => processorB.Process(processorA.Process(input));
}