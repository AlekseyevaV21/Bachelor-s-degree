using System;

public class PerlinNoise
{
    private int[] permutationTable;

    public PerlinNoise()
    {
        // Инициализация таблицы перестановок
        permutationTable = new int[512];
        Random random = new Random();

        // Заполняем таблицу значениями от 0 до 255
        for (int i = 0; i < 256; i++)
        {
            permutationTable[i] = i;
        }

        // Перетасовываем таблицу
        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            int temp = permutationTable[i];
            permutationTable[i] = permutationTable[j];
            permutationTable[j] = temp;
        }

        // Копируем первую половину в конец
        Array.Copy(permutationTable, 0, permutationTable, 256, 256);
    }

    // Функция для вычисления 2D шума Перлина
    public double Noise(double x, double y)
    {
        int ix = (int)Math.Floor(x) & 255;
        int iy = (int)Math.Floor(y) & 255;

        // Вводим псевдослучайные значения
        double fx = x - Math.Floor(x);
        double fy = y - Math.Floor(y);

        // Интерполяция
        double u = Fade(fx);
        double v = Fade(fy);

        // Псевдослучайные индексы для градиентов
        int a = permutationTable[ix] + iy;
        int b = permutationTable[ix + 1] + iy;
        int aa = permutationTable[a];
        int ab = permutationTable[a + 1];
        int ba = permutationTable[b];
        int bb = permutationTable[b + 1];

        // Интерполяция результатов
        double gradAA = Grad(aa, fx, fy);
        double gradAB = Grad(ab, fx, fy - 1.0);
        double gradBA = Grad(ba, fx - 1.0, fy);
        double gradBB = Grad(bb, fx - 1.0, fy - 1.0);

        double lerpX1 = Lerp(gradAA, gradBA, u);
        double lerpX2 = Lerp(gradAB, gradBB, u);

        return Lerp(lerpX1, lerpX2, v);
    }

    // Функция для вычисления плавного сглаживания
    private double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    // Линейная интерполяция
    private double Lerp(double a, double b, double t)
    {
        return a + t * (b - a);
    }

    // Градиент, использующий псевдослучайные числа
    private double Grad(int hash, double x, double y)
    {
        int h = hash & 15; // Убираем старшие биты
        double grad = 1.0 + (h & 7);  // Градиент в пределах от 1 до 8

        // Если старший бит равен 1, инвертируем знак
        if ((h & 8) != 0)
            grad = -grad;

        return grad * (x + y);  // Умножаем на (x + y) для использования в 2D
    }
}
