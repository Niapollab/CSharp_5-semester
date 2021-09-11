using System;
using System.Text;
using static System.Console;
using static System.Math;

IShape2DFactory factory = new RandomShape2DFactory();
while (true)
{
    Clear();
    Shape2D shape = factory.Create();
    Write(shape);
    ReadKey(true);
}

abstract record Shape2D
{
    public abstract double CalculateArea();

    public abstract double CalculatePerimeter();

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder
            .Append($"Area = { CalculateArea() }, ")
            .Append($"Perimeter = { CalculatePerimeter() }");
        return true;
    }
}

record Rectangle : Shape2D
{
    public double Width { get; }

    public double Height { get; }

    public Rectangle(double width, double height)
    {
        if (width <= 0)
            throw new ArgumentException("Длина должна быть положительной.", nameof(width));

        if (height <= 0)
            throw new ArgumentException("Ширина должна быть положительной.", nameof(height));

        Width = width;
        Height = height;
    }

    public override double CalculateArea()
        => Width * Height;

    public override double CalculatePerimeter()
        => 2 * (Width + Height);

    protected override bool PrintMembers(StringBuilder builder)
    {
        builder
            .Append($"{ nameof(Width) } = { Width }, ")
            .Append($"{ nameof(Height) } = { Height }, ");
        base.PrintMembers(builder);
        return true;
    }
}

record Ellipse : Shape2D
{
    public double SemiMajorAxis { get; }

    public double SemiMinorAxis { get; }

    public Ellipse(double semiMajorAxis, double semiMinorAxis)
    {
        if (semiMajorAxis <= 0)
            throw new ArgumentException("Большая полуось должна быть положительной.", nameof(semiMajorAxis));

        if (semiMinorAxis <= 0)
            throw new ArgumentException("Малая полуось должна быть положительной.", nameof(semiMinorAxis));
        
        SemiMajorAxis = semiMajorAxis;
        SemiMinorAxis = semiMinorAxis;
    }

    public override double CalculateArea()
        => PI * SemiMajorAxis * SemiMinorAxis;

    public override double CalculatePerimeter()
        => 4 * ((PI * SemiMajorAxis * SemiMinorAxis) + (SemiMajorAxis - SemiMinorAxis)) / (SemiMajorAxis + SemiMinorAxis);

    protected override bool PrintMembers(StringBuilder builder)
    {
        builder
            .Append($"{ nameof(SemiMajorAxis) } = { SemiMajorAxis }, ")
            .Append($"{ nameof(SemiMinorAxis) } = { SemiMinorAxis }, ");
        base.PrintMembers(builder);
        return true;
    }
}

record Square(double Width) : Rectangle(Width, Width);

record Circle(double SemiMajorAxis) : Ellipse(SemiMajorAxis, SemiMajorAxis);

interface IShape2DFactory
{
    Shape2D Create();
}

class RandomShape2DFactory : IShape2DFactory
{
    private const int MaxRandomValue = 100;

    private const int MinRandomValue = 0;

    private readonly Random _random;

    private static readonly Func<Random, Shape2D>[] _shapesGenerators = new Func<Random, Shape2D>[]
    {
        random => new Rectangle(random.Next(MinRandomValue, MaxRandomValue + 1), random.Next(MinRandomValue, MaxRandomValue + 1)),
        random => new Ellipse(random.Next(MinRandomValue, MaxRandomValue + 1), random.Next(MinRandomValue, MaxRandomValue + 1)),
        random => new Square(random.Next(MinRandomValue, MaxRandomValue + 1)),
        random => new Circle(random.Next(MinRandomValue, MaxRandomValue + 1))
    };

    public RandomShape2DFactory(Random random = default)
        => _random = random ?? new Random();

    public Shape2D Create()
    {
        Func<Random, Shape2D> generator = _shapesGenerators[_random.Next(0, _shapesGenerators.Length)];
        return generator(_random);
    }
}