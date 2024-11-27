using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KmlFilterAPI.models;
using KmlFilterAPI.services;
using Microsoft.AspNetCore.Mvc;

namespace KmlFilterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacemarkController : ControllerBase
    {
        private readonly IKmlService _kmlService;

        public PlacemarkController(IKmlService kmlService)
        {
            _kmlService = kmlService ?? throw new ArgumentNullException(nameof(kmlService));
        }

        /// <summary>
        /// Exporta os Placemarks filtrados em um novo arquivo KML.
        /// </summary>
        /// <param name="filters">Filtros aplicados.</param>
        /// <returns>Link para download do arquivo.</returns>
        [HttpPost]
        [Route("export")]
        public IActionResult ExportKml([FromBody] FilterModel filters)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Os filtros fornecidos são inválidos.");

                var filteredPlacemarks = _kmlService.FilterPlacemarks(filters);
                var filePath = _kmlService.ExportFilteredKml(filteredPlacemarks);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Message = "O arquivo KML não foi encontrado ou não pôde ser gerado." });
                }

                // Retornar o arquivo como um download
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);
                return File(fileBytes, "application/vnd.google-earth.kml+xml", fileName);
            }
            // catch (ArgumentException ex)
            // {
            //     return BadRequest(ex.Message);
            // }
            catch (Exception ex)
            {
                // Log a exceção aqui, se necessário
                return BadRequest("Erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// Retorna os Placemarks filtrados em formato JSON.
        /// </summary>
        /// <param name="filters">Filtros aplicados.</param>
        /// <returns>Lista de Placemarks filtrados.</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<PlacemarkModel>> GetFilteredPlacemarks([FromQuery] FilterModel filters)
        {
            try
            {
                var filteredPlacemarks = _kmlService.FilterPlacemarks(filters);
                return Ok(filteredPlacemarks);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log a exceção aqui, se necessário
                return BadRequest("Erro ao processar a solicitação.");
            }
        }

        /// <summary>
        /// Retorna os valores únicos disponíveis para os campos de filtragem.
        /// </summary>
        /// <returns>Valores únicos de CLIENTE, SITUAÇÃO e BAIRRO.</returns>
        [HttpGet]
        [Route("filters")]
        public IActionResult GetAvailableFilters()
        {
            try
            {
                var filters = new
                {
                    Clientes = _kmlService.GetUniqueValues("CLIENTE"),
                    Situacoes = _kmlService.GetUniqueValues("SITUAÇÃO"),
                    Bairros = _kmlService.GetUniqueValues("BAIRRO")
                };

                return Ok(filters);
            }
            catch (Exception ex)
            {
                // Log a exceção aqui, se necessário
                return BadRequest("Erro ao processar a solicitação.");
            }
        }
    }
}