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
        public string? Data { get; set; }
    }

    public class ResponseMessageLogin
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("exception")]
        public string? Exception { get; set; }

        [JsonPropertyName("data")]
        public UserInfo? Data { get; set; }
    }

    public class UserInfo
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
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
        public string? Exception { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public ResponseMessageObjectData()
        {
            Data = default(T);
        }
    }
}
