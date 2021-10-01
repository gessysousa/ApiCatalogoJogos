using ApiCatalogoJogos.Exceptions;
using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.Services;
using ApiCatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Controllers.v1
{
    [Route("api/V1/[controller]")]//rota -> controller fica entre colchetes para ser ignorado, quando for colocar o endereço no navegador, basta usar /Jogos
    [ApiController]
    public class JogosController : ControllerBase //jogosController herda controllerBase
    {
        private readonly IJogoService _jogoService; //propriedade, readonly porque não é de nossa responsabilidade dar uma instância pra ela, essa responsabilidade é do próprio asp.net 

        public JogosController(IJogoService jogoService) //construtor
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possível retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual página está sendo consultada. Mínimo 1</param>
        /// <param name="quantidade">Indica a quantidade de reistros por página. Mínimo 1 e máximo 50</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não haja jogos</response>  
        [HttpGet] //indicação do método get, lista todos os jogos
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5) //ActionResult é um tipo de retorno que vai indicar qual o status HTTP
        {                                                                                                                                                                   //variável página vem da query, ou seja da requisição na url
            var jogos = await _jogoService.Obter(pagina, quantidade);//vai chamar o serviço pedindo para obter a lista

            if (jogos.Count() == 0) //se não tiver nada na lista ele retorna que não tem contaúdo
                return NoContent();
            return Ok(jogos); //vai retornar status 200 e uma lista de objetos, no caso os jogos
        }

        /// <summary>
        /// Buscar um jogo pelo seu Id
        /// </summary>
        /// <param name="idJogo">Id do jogo buscado</param>
        /// <response code="200">Retorna o jogo filtrado</response>
        /// <response code="204">Caso não haja jogo com este id</response> 
        [HttpGet("{idJogo:guid}")] //indicação do método get de apenas um jogo
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo) //ActionResult é um tipo de retorno que vai indicar qual o status HTTP
        {                                                                             //FromRoute vem da rota do jogo especificado
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
                return NoContent();
            return Ok(jogo); //vai retornar uma lista de objetos
        }

        /// <summary>
        /// Inserir um jogo no catálogo
        /// </summary>
        /// <param name="jogoInputModel">Dados do jogo a ser inserido</param>
        /// <response code="200">Cao o jogo seja inserido com sucesso</response>
        /// <response code="422">Caso já exista um jogo com mesmo nome para a mesma produtora</response>  
        [HttpPost]//inserir um jogo
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)//vem do corpo da requisição, enviado viajson, transformado para dto
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);

                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora");
            }
        }

        /// <summary>
        /// Atualizar um jogo no catálogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="jogoInputModel">Novos dados para atualizar o jogo indicado</param>
        /// <response code="200">Cao o jogo seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>  
        [HttpPut("{idJogo:guid}")] //atualizar jogo, atualiza o recurso inteiro
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)//id do jogo vem da rota e as informações do jogo vem do body
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        /// <summary>
        /// Atualizar o preço de um jogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="preco">Novo preço do jogo</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>  
        [HttpPatch("{idJogo:guid}/preco/{preco:double}")] //atualizar apenas uma configuração do jogo e não o jogo inteiro
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)//informação vem da rota
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        /// <summary>
        /// Excluir um jogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser excluído</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>   
        [HttpDelete("{idJogo:guid}")] //deletar um jogo
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }


    }
}
