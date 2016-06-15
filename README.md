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

Multiple Widgets on the same page
====
Where the same widget is placed on the page multiple times, each needs to be able to be processed individually. This can be achieved by setting a `widgetId`:

```csharp
@await Widget.InvokeAsync("ContactForm", widgetId: "Form1");
@await Widget.InvokeAsync("ContactForm", widgetId: "Form2");
```

And when generating the form, add the `widget-form` or `widget-state` attributes:

```html
<form method="post" widget-form>

</form>
```

You would also need to add the tag helper:

```csharp
@addTagHelper *, Antaris.AspNetCore.Mvc.widgets
```

Multi-state Widgets
====

Often HTML workflows can span multiple pages using `<form>` to pass data from step to step. It is possible to model something similar using widgets. Widgets can support multiple states, e.g.

```csharp
public class WizardWidget : Widget
{
	public IWidgetResult InvokeGet()
	{
		return View("StepA");
	}

	public IWidgetResult InvokeStepAPost(WizardViewModel model)
	{
		if (ModelState.IsValid)
		{
			return View("StepB", model);
		}

		return View("StepA", model);
	}

	public IWidgetResult InvokeStepBPost(WizardViewModel model)
	{
		if (ModelState.IsValid)
		{
			return View("Confirmation", model);
		}

		return View("StepB");
	}
}
```

Which can be paired with a set of views:

```html
StepA.cshtml:
<form method="post" widget-state="StepA">
	...
</form>

StepB.cshtml:
<form method="post" widget-state="StepB">
	...
</form>
```

**THIS IS A PROTOTYPE**