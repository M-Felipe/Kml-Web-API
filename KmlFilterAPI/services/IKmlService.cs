using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KmlFilterAPI.models;

namespace KmlFilterAPI.services
{
    public interface IKmlService
    {
        IEnumerable<PlacemarkModel> FilterPlacemarks(FilterModel filters);
        IEnumerable<string> GetUniqueValues(string fieldName);
        string ExportFilteredKml(IEnumerable<PlacemarkModel> placemarks);
    }
}