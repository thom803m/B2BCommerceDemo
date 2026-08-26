namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        string BuildCompanyRegisteredTemplate(string companyName);
        string BuildCompanyApprovedTemplate(string companyName);
        string BuildCompanyRejectedTemplate(string companyName);

        string BuildOrderCreatedTemplate( int orderId, decimal total, DateTime createdAt);
        string BuildOrderProcessingTemplate(int orderId);
        string BuildOrderShippedTemplate(int orderId);
        string BuildOrderCompletedTemplate(int orderId);
        string BuildOrderCancelledTemplate(int orderId);

        string BuildEmailConfirmationTemplate(string confirmationLink);
        string BuildForgotPasswordTemplate(string resetLink);
    }
}

