#ifndef PIT_GEOMETRY
#define PIT_GEOMETRY

#define GEO_EPSILON 0.0001
#define GEO_INFINITY 1e30f

float Intersect_Ray_Plane(float3 rayPoint, float3 rayDir, float3 planePoint, float3 planeNormal)
{
    float t;
    
    float denom = dot(planeNormal, rayDir);
    if (abs(denom) > 0.0001)
    {
        t = dot(planeNormal, (planePoint - rayPoint)) / denom;
    }
    else
    {
        //If it is parallel, we return t = 0 so that the ray stays where
        // it is since it won't collide against the plane
        t = 0.0;
    }
    
    return t;
}

float3 GetIntersection_Ray_Plane(float3 rayPoint, float3 rayDir, float3 planePoint, float3 planeNormal)
{
    float t = Intersect_Ray_Plane(rayPoint, rayDir, planePoint, planeNormal);
    return rayPoint + rayDir * t;
}

//It is assumed that the box is at (0,0,0) with size (1,1,1)
float3 GetIntersection_Ray_AABB_OS(float3 viewPosOS, float3 viewDirOS)
{
    float3 side_x = float3(viewDirOS.x < 0 ? 0.5 : -0.5, 0.0, 0.0);
    float3 side_y = float3(0.0, viewDirOS.y < 0 ? 0.5 : -0.5, 0.0);
    float3 side_z = float3(0.0, 0.0, viewDirOS.z < 0 ? 0.5 : -0.5);

    float t_x = Intersect_Ray_Plane(viewPosOS, viewDirOS, side_x, side_x);
    float t_y = Intersect_Ray_Plane(viewPosOS, viewDirOS, side_y, side_y);
    float t_z = Intersect_Ray_Plane(viewPosOS, viewDirOS, side_z, side_z);

    float t = max(max(0.0001, t_x), max(t_y, t_z));
    return viewPosOS + viewDirOS * t;
}

/*
    Given a normal in world space and a depth threshold, it assumes
    that the threshold is intended for surfaces orthogonal to the
    camera forward direction. This functions returns the hypothenuse
    of a triangle formed by the threshold as the adjacent side whose
    length equals the threshold, and a hypothenuse whose direction
    is the forward vector of the camera.

    This means that the threshold will become greater as the normal points away
    from the camera.
*/
float CalculateDepthThreshold(float3 normalWS, float linearThreshold)
{
    //return linearThreshold;
    
    float3 normalVS = TransformWorldToViewNormal(normalWS, false);
    //float cos_alpha = dot(normalVS, float3(0, 0, 1));
    float cos_alpha = abs(normalVS.z); //dot(normalVS, float3(0,0,1))
    float hyp = linearThreshold * rcp(cos_alpha + GEO_EPSILON);
    
    
    
    
    return hyp;
}



#endif