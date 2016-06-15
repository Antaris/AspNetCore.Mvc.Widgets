using Antaris.AspNetCore.Mvc.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleWidgetApplication.Widgets
{
    public class WizardWidget : Widget
    {
        public IWidgetResult InvokeGet()
        {
            return View("Question", new WizardViewModel());
        }

        public IWidgetResult InvokeQuestionPost(WizardViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("Confirmation", model);
            }
            else
            {
                return View("Question", model);
            }
        }

        public IWidgetResult InvokeConfirmationPost(WizardViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("Answer", model);
            }
            else
            {
                return View("Confirmation", model);
            }
        }
    }

    public class WizardViewModel
    {
        [Required]
        public string Name { get; set; }

        public bool DoYouLikeStuff { get; set; }
    }
}
