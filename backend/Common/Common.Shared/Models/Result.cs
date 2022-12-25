using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Shared.Models
{
    public class Result
    {
        public bool Succeeded { get; set; }

        public List<string> Errors { get; set; }

        public Object ObjectReturn { get; set; }

        internal Result(bool succeeded, List<string> errors, Object objectReturn = null)
        {
            Succeeded = succeeded;
            Errors = errors;
            ObjectReturn = objectReturn;

        }
        public string GetError()
        {
            string error = "";
            for (int i = 0; i < Errors.Count; i++)
            {
                error += Errors[i] + "\r\n";
            }
            return error;
        }
        public static Result Success(Object objectReturn = null)
        {
            return new Result(true, new List<string> { }, objectReturn);
        }

        public static Result Failure(List<string> errors, Object objectReturn = null)
        {
            return new Result(false, errors, objectReturn);
        }
        public static Result Failure(params string[] paramData)
        {
            return new Result(false, paramData.ToList(), null);
        }


        public static implicit operator Result(int v)
        {
            throw new NotImplementedException();
        }

        public static T ReadContentAs<T>(object jsonObject)
        {
            if (string.IsNullOrEmpty(jsonObject.ToString()))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(jsonObject.ToString());
        }
    }
}
