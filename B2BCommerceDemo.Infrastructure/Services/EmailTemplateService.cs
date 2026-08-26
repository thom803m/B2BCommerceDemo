using B2BCommerceDemo.Core.Interfaces.Services;
using System.Text;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string BuildCompanyRegisteredTemplate(string companyName)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>New company registration</h1>

<p>A new company has registered.</p>

<p><strong>Company:</strong> {companyName}</p>

</div>

</body>
</html>";
        }

        public string BuildCompanyApprovedTemplate(string companyName)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>Company approved</h1>

<p>Your company <strong>{companyName}</strong> has been approved.</p>

<p>You can now log in and use the webshop.</p>

</div>

</body>
</html>";
        }

        public string BuildCompanyRejectedTemplate(string companyName)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>Company registration rejected</h1>

<p>Unfortunately your registration for <strong>{companyName}</strong> was rejected.</p>

<p>Please contact support for more information.</p>

</div>

</body>
</html>";
        }



        public string BuildOrderCreatedTemplate(
            int orderId,
            decimal total,
            DateTime createdAt)
        {
            var sb = new StringBuilder();

            sb.Append($@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1 style='color:#222;'>Thanks for your order</h1>

<p>Your order has been confirmed.</p>

<hr/>

<p><strong>Order ID:</strong> #{orderId}</p>
<p><strong>Total:</strong> {total:C}</p>
<p><strong>Date:</strong> {createdAt:u}</p>

<hr/>

<p>We’ll notify you again when your order ships.</p>

</div>

</body>
</html>");

            return sb.ToString();
        }

        public string BuildOrderProcessingTemplate(int orderId)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>Order is being processed</h1>

<p>Your order <strong>#{orderId}</strong> is currently being prepared.</p>

<p>We'll notify you again when it ships.</p>

</div>

</body>
</html>";
        }

        public string BuildOrderShippedTemplate(int orderId)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>Your order is on the way.</h1>

<p>Your order <strong>#{orderId}</strong> has been shipped.</p>

<p>Thank you for shopping with us.</p>

</div>

</body>
</html>";
        }

        public string BuildOrderCompletedTemplate(int orderId)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>Order completed!</h1>

<p>Your order <strong>#{orderId}</strong> has been completed.</p>

<p>We hope you enjoy your purchase.</p>

</div>

</body>
</html>";
        }

        public string BuildOrderCancelledTemplate(int orderId)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>Order cancelled.</h1>

<p>Your order <strong>#{orderId}</strong> has been cancelled.</p>

<p>If you have questions, please contact support.</p>

</div>

</body>
</html>";
        }



        public string BuildEmailConfirmationTemplate(string confirmationLink)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>

<h1>Confirm your email</h1>

<p>Thank you for registering.</p>

<p>Please confirm your email address to activate your account.</p>

<p>
    <a href='{confirmationLink}'>
        Confirm Email
    </a>
</p>

</div>

</body>
</html>";
        }

        public string BuildForgotPasswordTemplate(string resetLink)
        {
            return $@"
<html>
<body style='font-family: Arial; background:#f4f4f4; padding:20px;'>

<div style='max-width:600px; margin:auto; background:white; padding:30px;'>

<h1>Password reset</h1>

<p>You requested a password reset.</p>

<a href='{resetLink}'>Reset password</a>

</div>

</body>
</html>";
        }
    }
}
