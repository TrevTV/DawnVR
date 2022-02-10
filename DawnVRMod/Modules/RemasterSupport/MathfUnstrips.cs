namespace UnityEngine
{
    internal static class MathfUnstrips
    {
        // Taken from the mono DLL's Mathf class
        public const float Deg2Rad = 0.0174532924f;
        public const float Rad2Deg = 57.29578f;
        public const float PI = 3.14159274f;

		public static float Max(float a, float b)
		{
			return (a <= b) ? b : a;
		}

		public static float Max(params float[] values)
		{
			int num = values.Length;
			float result;
			if (num == 0)
			{
				result = 0f;
			}
			else
			{
				float num2 = values[0];
				for (int i = 1; i < num; i++)
				{
					if (values[i] > num2)
					{
						num2 = values[i];
					}
				}
				result = num2;
			}
			return result;
		}

		public static int Max(int a, int b)
		{
			return (a <= b) ? b : a;
		}

		public static int Max(params int[] values)
		{
			int num = values.Length;
			int result;
			if (num == 0)
			{
				result = 0;
			}
			else
			{
				int num2 = values[0];
				for (int i = 1; i < num; i++)
				{
					if (values[i] > num2)
					{
						num2 = values[i];
					}
				}
				result = num2;
			}
			return result;
		}
	}
}
