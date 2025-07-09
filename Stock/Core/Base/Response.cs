using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Base
{
    public class Response<T> : IResponseBase
    {
        public Response()
        {
        }

        protected Response(T data)
        {
            Data = data;
        }

        protected Response(ValidationError error)
        {
            Error = error;
        }

        public ValidationError Error { get; protected set; }

        public T Data { get; set; }

        private int _statusCode;
        public int StatusCode
        {
            get => _statusCode > 0 ? _statusCode : !string.IsNullOrEmpty(ErrorMessage) ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;
            set => _statusCode = value;
        }

        private bool _success;
        public bool Success
        {
            set => _success = value;
            get => string.IsNullOrEmpty(ErrorMessage) || _success;
        }
        private string _errorMessage;
        public string ErrorMessage
        {
            get => !string.IsNullOrEmpty(_errorMessage) ? _errorMessage : string.Empty;
            set => _errorMessage = value;
        }

        public static implicit operator Response<T>(T result)
        {
            return new Response<T>(result);
        }

        public static implicit operator Response<T>(ValidationError error)
        {
            return new Response<T>(error);
        }
    }

    public class ValidationError
    {
        public static ValidationError NotFound(string message) => new ValidationError((int)HttpStatusCode.NotFound, message);
        public static ValidationError Unauthorized(string message) => new ValidationError((int)HttpStatusCode.Unauthorized, message);
        public static ValidationError Forbidden(string message) => new ValidationError((int)HttpStatusCode.Forbidden, message);
        public static ValidationError InternalServerError(string message) => new ValidationError((int)HttpStatusCode.InternalServerError, message);
        public static ValidationError BadRequest(string message) => new ValidationError((int)HttpStatusCode.BadRequest, message);
        public static ValidationError Conflict(string message) => new ValidationError((int)HttpStatusCode.Conflict, message);
        public static ValidationError Ambiguous(string message) => new ValidationError((int)HttpStatusCode.Ambiguous, message);

        public ValidationError(int statusCode, string details = null, string additionalDataJson = null)
        {
            Details = details;
            StatusCode = statusCode;
            AdditionalDataJson = additionalDataJson;
        }

        public string Details { get; }

        public int StatusCode { get; }

        public string AdditionalDataJson { get; }
    }
}
