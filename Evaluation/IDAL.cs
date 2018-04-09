using MongoRepository.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evaluation
{
    public interface IDAL
    {
        DiseasesData GetRealData();

        DiseasesData GetPredictionData();
    }
}
