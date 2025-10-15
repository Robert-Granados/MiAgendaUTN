using System;
using System.ComponentModel.DataAnnotations;

namespace MiAgendaUTN.ViewModels
{
    public class ActivityModel
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [FechaNoPasada]
        public DateTime Fecha { get; set; } = DateTime.Today;



        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int Categoria { get; set; }

        public class FechaNoPasadaAttribute : ValidationAttribute
        {
            public FechaNoPasadaAttribute()
            {
                ErrorMessage = "La fecha no puede ser anterior a hoy.";
            }
            public override bool IsValid(object? value)
            {
                if (value is DateTime fecha)
                {
                    return fecha >= DateTime.Today;
                }
                return false;
            }
        }
    }
}