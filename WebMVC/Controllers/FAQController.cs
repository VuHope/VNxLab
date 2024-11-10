using Microsoft.AspNetCore.Mvc;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class FAQController : Controller
    {
        public IActionResult Index()
        {


            var model = new List<QuestionAnswer>
        {
            new QuestionAnswer { QuestionText = "Làm thế nào để đăng sản phẩm mới?", AnswerText = "Bạn cần đăng nhập vào tài khoản của mình, sau đó chọn \"Đăng sản phẩm\". Điền đầy đủ thông tin sản phẩm, bao gồm tên, mô tả, giá cả, và hình ảnh, rồi nhấn \"Đăng\" để hoàn tất." },
            new QuestionAnswer { QuestionText = "Tại sao sản phẩm của tôi chưa hiển thị sau khi đăng?", AnswerText = "Sản phẩm của bạn có thể đang được kiểm duyệt. Thời gian kiểm duyệt thường mất từ 1 đến 24 giờ. Khi sản phẩm được chấp nhận, nó sẽ hiển thị trên trang web." },
            new QuestionAnswer{QuestionText = "Làm cách nào để chỉnh sửa sản phẩm đã đăng?", AnswerText="Vào mục \"Sản phẩm của tôi\" trong tài khoản, chọn sản phẩm cần chỉnh sửa, nhấp vào nút \"Chỉnh sửa\", thực hiện các thay đổi và lưu lại."},
            new QuestionAnswer{QuestionText = "Làm sao để xóa sản phẩm đã đăng?", AnswerText="Vào mục \"Sản phẩm của tôi\", chọn sản phẩm cần xóa và nhấn nút \"Xóa\". Sản phẩm sẽ bị xóa khỏi trang ngay lập tức."},
            new QuestionAnswer{QuestionText = "Tại sao tài khoản của tôi bị khóa?", AnswerText="Tài khoản của bạn có thể đã vi phạm chính sách của trang web, như đăng nội dung không phù hợp hoặc lừa đảo. Hãy liên hệ với bộ phận hỗ trợ để được hỗ trợ thêm."},
            new QuestionAnswer{QuestionText = "Tôi có thể đăng bao nhiêu sản phẩm cùng lúc?", AnswerText="Số lượng sản phẩm bạn có thể đăng phụ thuộc vào loại tài khoản của bạn. Tài khoản miễn phí có thể bị giới hạn, trong khi tài khoản trả phí có thể đăng số lượng lớn hơn."},
            new QuestionAnswer{QuestionText = "Tôi cần cung cấp thông tin nào để đăng ký tài khoản?", AnswerText="Để đăng ký tài khoản, bạn cần cung cấp tên, địa chỉ email hợp lệ, số điện thoại, và mật khẩu. Sau khi điền thông tin, bạn sẽ nhận được email xác nhận để kích hoạt tài khoản."},
           
            // Thêm các câu hỏi và câu trả lời khác ở đây
        };
            ViewData["FAQModel"] = model;
            return View();
        }
    }
}
