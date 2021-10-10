namespace Odco.PointOfSales.Application.GeneralDto
{
    public class ResponseDto<T> where T : class
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public T[] Items { get; set; }
    }
}
