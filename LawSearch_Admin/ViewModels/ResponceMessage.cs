using LawSearch_Core.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;
using System.Text.Json;

namespace LawSearch_Admin.ViewModels
{
    public class ResponseMessage
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; } = null;

        public bool StatusAPI { get; set; } = false;

        [JsonPropertyName("message")]
        public string? Message { get; set; } = null;

        public string? Error { get; set; } = null;

        [JsonPropertyName("exception")]
        public string? Exception { get; set; } = null;

        [JsonPropertyName("data")]
        public string? Data { get; set; } = null;

        public ResponseMessage()
        {

        }
    }

    public class ResponseMessageLogin
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; } = null;

        [JsonPropertyName("message")]
        public string? Message { get; set; } = null;

        [JsonPropertyName("exception")]
        public string? Exception { get; set; } = null;

        [JsonPropertyName("data")]
        public UserInfo? Data { get; set; } = null;
    }

    public class UserInfo
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; } = null;

        [JsonPropertyName("username")]
        public string? Username { get; set; } = null;

        [JsonPropertyName("role")]
        public string? Role { get; set; } = null;

        [JsonPropertyName("token")]
        public string? Token { get; set; } = null;
    }

    public class ResponseMessageListData<T>
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; } = null;

        public bool StatusAPI { get; set; } = false;

        [JsonPropertyName("message")]
        public string? Message { get; set; } = null;

        public string? Error { get; set; } = null;

        [JsonPropertyName("exception")]
        public string? Exception { get; set; } = null;

        [JsonPropertyName("data")]
        public List<T> ListData { get; set; }

        public ResponseMessageListData()
        {
            ListData = new List<T>();
        }
    }

    public class ResponseMessageObjectData<T>
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; } = null;

        public bool StatusAPI { get; set; } = false;

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("exception")]
        public string? Exception { get; set; } = null;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public ResponseMessageObjectData()
        {
            Data = default(T);
        }
    }
}
