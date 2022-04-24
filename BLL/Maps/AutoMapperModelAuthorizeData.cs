using System.Collections.Generic;
using Domain.Entities;

namespace BLL.Maps;

public class AutoMapperModelAuthorizeData
{
    public User UserComparable { get; private set; }
    public User UserCompared { get; private set; }

    public Dictionary<string, AutoMapperModelFieldAuthorizeData> AutoMapperModelFieldAuthorizeDatas
    {
        get;
        private set;
    }

    public AutoMapperModelAuthorizeData(
        User userComparable,
        User userCompared,
        Dictionary<string, AutoMapperModelFieldAuthorizeData> autoMapperModelFieldAuthorizeDatas
    )
    {
        UserComparable = userComparable;
        UserCompared = userCompared;
        AutoMapperModelFieldAuthorizeDatas = autoMapperModelFieldAuthorizeDatas;
    }
}