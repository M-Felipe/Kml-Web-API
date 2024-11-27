using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KmlFilterAPI.models;

namespace KmlFilterAPI.services
{
    public interface IKmlService
    {
        /// <summary>
        /// Filtra os itens do Placemarks de acordo com os filtros informados.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        IEnumerable<PlacemarkModel> FilterPlacemarks(FilterModel filters);

        /// <summary>
        /// Retorna os valores únicos de um campo específico.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        IEnumerable<string> GetUniqueValues(string fieldName);

        /// <summary>
        /// Exporta os Placemarks filtrados para um arquivo KML que é salvo na pasta temporaria do servidor.
        /// </summary>
        /// <param name="placemarks"></param>
        /// <returns></returns>
        string ExportFilteredKml(IEnumerable<PlacemarkModel> placemarks);
    }
}