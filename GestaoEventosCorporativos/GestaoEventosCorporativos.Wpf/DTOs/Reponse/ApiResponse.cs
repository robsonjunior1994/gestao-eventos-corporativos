﻿namespace GestaoEventosCorporativos.Wpf.DTOs.Reponse
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public T Data { get; set; }
        public object Errors { get; set; }
    }
}
