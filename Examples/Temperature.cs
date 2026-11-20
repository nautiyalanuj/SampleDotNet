namespace FirstApplication.Examples
{
    /// <summary>
    /// Sample class: holds state (a property), validates it in the constructor,
    /// and exposes behaviour through methods.
    /// </summary>
    public class Temperature
    {
        public Temperature(double celsius)
        {
            if (celsius < -273.15)
            {
                throw new ArgumentOutOfRangeException(nameof(celsius), "Below absolute zero.");
            }

            Celsius = celsius;
        }

        public double Celsius { get; }

        // Sample function: takes no input beyond the instance and returns a value.
        public double ToFahrenheit()
        {
            return Celsius * 9 / 5 + 32;
        }

        // Sample static function: pure, needs no instance.
        public static Temperature FromFahrenheit(double fahrenheit)
        {
            return new Temperature((fahrenheit - 32) * 5 / 9);
        }

        public override string ToString()
        {
            return $"{Celsius:F1} C / {ToFahrenheit():F1} F";
        }
    }
}
