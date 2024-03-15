namespace LawSearch_Core.Models
{

    public class MessageResponseConstants
    {
        private static readonly Dictionary<MessageKey, MessageModel> messageModels = new Dictionary<MessageKey, MessageModel>
        {
            //Any
            {MessageKey.SUCCESS, new MessageModel(200, "OK", "Xử lí thành công", "Any") },
            {MessageKey.FORBIDDEN, new MessageModel(201, "There is no access to this feature",
                                                                            "Không có quyền truy cập vào tính năng này (Gửi yêu cầu quyền)", "Any")},
            {MessageKey.INVALID_TOKEN, new MessageModel(202, "Invalid token code", "Mã xác thực không hợp lệ (Kiểm tra Token ở Header)", "Any")},
            {MessageKey.TOKEN_NOT_FOUND, new MessageModel(203, "Token code not found", "Không tìm thấy mã xác thực (Kiểm tra Token ở Header)", "Any")},
            {MessageKey.BAD_PARAM, new MessageModel(204, "The input parameter is incomplete", "Tham số đầu vào không đầy đủ (Kiểm tra các tham số truyền lên)", "Any")},
            {MessageKey.BAD_REQUEST, new MessageModel(205, "An exception error occurred", "Xảy ra lỗi ngoại lệ (Kiểm tra Exception trả về)", "Any")},
            {MessageKey.PARTNERKEY_NOT_FOUND, new MessageModel(206, "PartnerKey not found", "Không tìm thấy mã đối tác (Kiểm tra PartnerKey ở Header)", "Any")},
        };
        public static string GetMessage(MessageKey key, string? sub = null)
        {
            if (messageModels.TryGetValue(key, out var messageModel))
            {
                return messageModel.Message + sub;
            }
            throw new InvalidOperationException("Key not found");
        }
    }

    public enum MessageKey
    {
        SUCCESS,
        FORBIDDEN,
        INVALID_TOKEN,
        TOKEN_NOT_FOUND,
        BAD_PARAM,
        BAD_REQUEST,
        PARTNERKEY_NOT_FOUND
    }
}
