using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundTemperatures.Models
{
    public class DailyMeasurement
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MeasurementValue abs_scaled_paw_soil_moisture_0to10cm { get; set; }

        public MeasurementValue abs_scaled_paw_soil_moisture_0to200cm { get; set; }

        public MeasurementValue abs_scaled_soil_moisture_0to10cm { get; set; }

        public MeasurementValue abs_scaled_soil_moisture_0to200cm { get; set; }

        public MeasurementValue normalized_soil_moisture_0to10cm { get; set; }

        public MeasurementValue normalized_soil_moisture_0to200cm { get; set; }

        public MeasurementValue normalized_soil_temp_0to10cm { get; set; }

        public MeasurementValue scaled_soil_moisture_0to10cm { get; set; }

        public MeasurementValue scaled_soil_moisture_0to200cm { get; set; }

        public MeasurementValue soil_moisture_0to10cm { get; set; }

        public MeasurementValue soil_moisture_0to200cm { get; set; }

        public MeasurementValue soil_temp_0to10cm { get; set; }

        public MeasurementValue soil_temp_max_0to10cm { get; set; }

        public MeasurementValue soil_temp_min_0to10cm { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
