ASP.NET Core MVC - Widgets
===

A prototype Widget execution framework based off of the design of MVC ViewComponents.

Widgets are essentially the same as ViewComponents with some subtle differences:

1. The arguments provided to a Widget can be provided through model binding
2. The methods that are executed are determined based on the HTTP method.

Example Widget
====

```csharp
using Antaris.AspNetCore.Mvc.Widgets;

public class ContactFormWidget : Widget
{
  public IWidgetResult InvokeGet()
  {
    return View("Form", new ContactFormViewModel());
  }
  
  public async Task<IWidgetResult> InvokePostAsync(ContactFormViewModel model, [FromServices] IEmailSender emailSender)
  {
    if (ModelState.IsValid)
    {
      await emailSender.SendEmailAsync(model.Email, "Thank you", "Thank you for your enquiry.");
      
      return View("Confirmation", model);
    }
    
    return View("Form", model);
  }
}
```

Then you can call your Widget from your view:

```csharp
@inject Antaris.AspNetCore.Mvc.Widgets.IWidgetHelper Widget

@await Widget.InvokeAsync("ContactForm")
```

Arguments can be provided through the model binder, or optionall you can pass them in as an anonymous object, e.g.:

```csharp
@await Widget.InvokeAsync("ContactForm", new { product })

//...
  public IWidgetResult InvokeGet(Product product)
  {
  
  }
```

Arguments provided at invocation are used over model binder-sourced arguments.

Widgets can be used to create isolated units of logic that can render and respond independently of the parent page.

**THIS IS A PROTOTYPE**