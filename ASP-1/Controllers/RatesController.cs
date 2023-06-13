using ASP_1.Data;
using ASP_1.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_1.Controllers
{
    [Route("api/rates")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public RatesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public object Get([FromQuery] String data)
        {
            return new { result = $"Запрос обработан с помощью Get: {data}" };
        }

        [HttpDelete]
        public object Delete([FromBody] BodyData bodyData)
        {
            int statusCode;


            string result = $"Запрос обработан с помощью Delete: {bodyData.Data}";

            if (bodyData == null || bodyData.Data == null || bodyData.ItemId == null || bodyData.UserId == null)
            {
                statusCode = StatusCodes.Status400BadRequest;
                result = $"Не все данные переданы: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
            }
            else
            {
                try
                {
                    Guid itemId = Guid.Parse(bodyData.ItemId);
                    Guid userId = Guid.Parse(bodyData.UserId);
                    int rating = Convert.ToInt32(bodyData.Data);

                    Rate? rate = _dataContext.Rates.FirstOrDefault(r => r.UserId == userId && r.ItemId == itemId);
                    if (rate is not null)
                    {
                        _dataContext.Rates.Remove(rate);
                        _dataContext.SaveChanges();
                        statusCode = StatusCodes.Status202Accepted;
                        result = $"Данные удалены: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                    }
                    else
                    {
                        statusCode = StatusCodes.Status406NotAcceptable;
                        result = $"Данные уже явны и не могут быть удаленны: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                        _dataContext.Rates.Add(new()
                        {
                            ItemId = itemId,
                            UserId = userId,
                            Rating = rating
                        });
                        _dataContext.SaveChanges();
                        statusCode = StatusCodes.Status201Created;
                        result = $"Данные внесено: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                    }
                }
                catch
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    result = $"Данные не обработаны: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                    throw;
                }
            }
            HttpContext.Response.StatusCode = statusCode;
            return new { result };
        }

        [HttpPost]
        public object Post([FromBody] BodyData bodyData)
        {
            int statusCode;

           
            string result = $"Запрос обработан с помощью Post: {bodyData.Data}";

            if (bodyData == null || bodyData.Data == null || bodyData.ItemId == null || bodyData.UserId == null)
            {
                statusCode = StatusCodes.Status400BadRequest;
                 result = $"Не все данные переданы: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
            }
            else
            {
                try
                {
                    Guid itemId = Guid.Parse(bodyData.ItemId);
                    Guid userId = Guid.Parse(bodyData.UserId);
                    int rating = Convert.ToInt32(bodyData.Data);
                    Rate? rate = _dataContext.Rates.FirstOrDefault(r => r.UserId == userId && r.ItemId == itemId);
                    if (rate is not null)
                    {
                        if(rate.Rating == rating)
                        {
                            statusCode = StatusCodes.Status406NotAcceptable;
                            result = $"Данные уже явны: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                        }
                        else
                        {
                            rate.Rating = rating;
                            _dataContext.SaveChanges();
                            statusCode = StatusCodes.Status202Accepted;
                            result = $"Данные обновлены: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                        }
                       
                    }
                    else
                    {
                        _dataContext.Rates.Add(new()
                        {
                            ItemId = itemId,
                            UserId = userId,
                            Rating = rating
                        });
                        _dataContext.SaveChanges();
                        statusCode = StatusCodes.Status201Created;
                        result = $"Данные внесено: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                    }
                }
                catch
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    result = $"Данные не обработаны: Data = {bodyData?.Data} ItemId = {bodyData?.ItemId} UserId = {bodyData?.UserId}";
                    throw;
                }
            }
            HttpContext.Response.StatusCode = statusCode;
            return new { result };
        }


    }

    public class BodyData
    {
        public String? Data { get; set; }
        public String? ItemId { get; set; } 
        public String? UserId { get; set; } 

    }

}
