import numpy as np
from datetime import datetime

time_delta = 8.0
latitude = 1.3521
longitude = 103.8198
day_of_year = datetime.now().timetuple().tm_yday

# 1. Declination angle
declination_angle = 23.45 * (np.pi / 180) * np.sin((np.pi * 2 * ((284 + day_of_year) / 36.25)))

# 3. Azimuth angle
azimuth_angle = np.arcsin(
    () / np.sin(altitude_angle))

# 4. Hour angle
hour_angle = np.arcsin(
    np.sin(altitude_angle)
)

# 2. Altitude angle
altitude_angle = np.arcsin(
    (np.sin(declination_angle) * np.sin(latitude)) + 
    (np.cos(declination_angle) * np.cos(hour_angle) * np.cos(latitude)))

# 5. Angle of incidence
