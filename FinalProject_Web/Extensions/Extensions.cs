namespace FinalProject_Web.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Публичный статический метод невозвращаемого типа,
        /// который применяется к переменной любого типа T,
        /// являющейся массивом данных. Данный метод увеличивает
        /// массив данных на 1 элемент.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Col"></param>
        public static T[] Resize<T>(this T[] Col)
        {
            Array.Resize(ref Col, Col.Length + 1);
            return Col;
        }

        ///// <summary>
        ///// Публичный статический метод невозвращаемого типа,
        ///// который применяется к переменной любого типа U,
        ///// являющейся массивом данных. Данный метод увеличивает
        ///// массив данных. При условии длины массива = 0 массив
        ///// увеличится до 4 элементов, в противном случае -
        ///// массив увеличится вдвое.
        ///// </summary>
        ///// <typeparam name="U"></typeparam>
        ///// <param name="Col"></param>
        public static U[] ResizeMultiple<U>(this U[] Col)
        {
            Array.Resize(ref Col, Col.Length == 0 ? 4 : Col.Length * 2);
            return Col;
        }

        /// <summary>
        /// Публичный статический метод невозвращаемого типа,
        /// который применяется к переменной любого типа U,
        /// являющейся массивом данных и принимает в себя int
        /// значение длинны текущего массива collectionSize. 
        /// Данный метод увеличивает массив данных. При условии 
        /// длины массива = 0 массив увеличится до 4 элементов, 
        /// в противном случае - массив увеличится вдвое.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="Col"></param>
        /// <param name="collectionSize"></param>
        public static U[] ResizeMultiple<U>(this U[] Col, int collectionSize)
        {
            Array.Resize(ref Col, collectionSize == 0 ? 4 : Col.Length * 2);
            return Col;
        }
    }
}
