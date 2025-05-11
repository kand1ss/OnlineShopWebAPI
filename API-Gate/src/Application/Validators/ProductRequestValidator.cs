using System.ComponentModel.DataAnnotations;

namespace APIGate.Application.Validators;

public class ProductRequestValidator
{
    public bool Validate<T>(T model, out IEnumerable<string> errors)
    {
        if (model == null)
        {
            errors = ["Model cannot be null."];
            return false;
        }
        
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        var validationResult = Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        
        errors = results
            .Where(r => !string.IsNullOrWhiteSpace(r.ErrorMessage))
            .Select(r => r.ErrorMessage!)
            .ToList();

        return validationResult;
    }
}