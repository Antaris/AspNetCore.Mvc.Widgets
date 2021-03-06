﻿using Antaris.AspNetCore.Mvc.Widgets;
using Microsoft.AspNetCore.Mvc;
using SampleWidgetApplication.Services;
using SampleWidgetApplication.Models;
using System.Threading.Tasks;

namespace SampleWidgetApplication.Widgets
{
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
                await emailSender.SendEmailAsync(model.Email, "Thank You", "Thank you for your enquiry, we'll get back to you when we can.");

                return View("Confirmation", model);
            }

            return View("Form", model);
        }
    }
}