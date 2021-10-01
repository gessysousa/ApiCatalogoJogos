using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.InputModel
{
    public class JogoInputModel
    {   //fail fast validation
        [Required] //data notation é um maneira de você decorar um atributo e ele valida o model state e isso é feito no próprio middleware do asp.net core
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome do jogo deve conter entre 3 e 100 caracteres")] //se não seguir aessas regras que foram definidas, automaticamente a mensagem de erro
        public string Nome { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome da produtora deve conter entre 3 e 100 caracteres")]
        public string Produtora { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "O preço deve ser de, no mínimo, R$ 1,00 e, no máximo, R$ 1000,00")]
        public double Preco { get; set; }

    }
}
