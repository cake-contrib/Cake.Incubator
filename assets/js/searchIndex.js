
var camelCaseTokenizer = function (obj) {
    var previous = '';
    return obj.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
        var current = cur.toLowerCase();
        if(acc.length === 0) {
            previous = current;
            return acc.concat(current);
        }
        previous = previous.concat(current);
        return acc.concat([current, previous]);
    }, []);
}
lunr.tokenizer.registerFunction(camelCaseTokenizer, 'camelCaseTokenizer')
var searchModule = function() {
    var idMap = [];
    function y(e) { 
        idMap.push(e); 
    }
    var idx = lunr(function() {
        this.field('title', { boost: 10 });
        this.field('content');
        this.field('description', { boost: 5 });
        this.field('tags', { boost: 50 });
        this.ref('id');
        this.tokenizer(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
    });
    function a(e) { 
        idx.add(e); 
    }

    a({
        id:0,
        title:"ProjectParserExtensions",
        content:"ProjectParserExtensions",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"SolutionParserExtensions",
        content:"SolutionParserExtensions",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"CustomProjectParserResult",
        content:"CustomProjectParserResult",
        description:'',
        tags:''
    });

    a({
        id:3,
        title:"GlobbingExtensions",
        content:"GlobbingExtensions",
        description:'',
        tags:''
    });

    a({
        id:4,
        title:"AssertExtensions",
        content:"AssertExtensions",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"FileSystemExtensions",
        content:"FileSystemExtensions",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"BuildTargetExecutable",
        content:"BuildTargetExecutable",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"ProjectPathExtensions",
        content:"ProjectPathExtensions",
        description:'',
        tags:''
    });

    a({
        id:8,
        title:"PackageReference",
        content:"PackageReference",
        description:'',
        tags:''
    });

    a({
        id:9,
        title:"EnumerableExtensions",
        content:"EnumerableExtensions",
        description:'',
        tags:''
    });

    a({
        id:10,
        title:"ProjectTypes",
        content:"ProjectTypes",
        description:'',
        tags:''
    });

    a({
        id:11,
        title:"StringExtensions",
        content:"StringExtensions",
        description:'',
        tags:''
    });

    a({
        id:12,
        title:"LoggingExtensions",
        content:"LoggingExtensions",
        description:'',
        tags:''
    });

    a({
        id:13,
        title:"ProjectType",
        content:"ProjectType",
        description:'',
        tags:''
    });

    a({
        id:14,
        title:"RuntimeOptions",
        content:"RuntimeOptions",
        description:'',
        tags:''
    });

    a({
        id:15,
        title:"DirectoryExtensions",
        content:"DirectoryExtensions",
        description:'',
        tags:''
    });

    a({
        id:16,
        title:"DotNetBuildSettingsExtensions",
        content:"DotNetBuildSettingsExtensions",
        description:'',
        tags:''
    });

    a({
        id:17,
        title:"DotNetCliToolReference",
        content:"DotNetCliToolReference",
        description:'',
        tags:''
    });

    a({
        id:18,
        title:"NetCoreProject",
        content:"NetCoreProject",
        description:'',
        tags:''
    });

    a({
        id:19,
        title:"CustomProjectFile",
        content:"CustomProjectFile",
        description:'',
        tags:''
    });

    a({
        id:20,
        title:"XDocumentExtensions",
        content:"XDocumentExtensions",
        description:'',
        tags:''
    });

    a({
        id:21,
        title:"EnvironmentExtensions",
        content:"EnvironmentExtensions",
        description:'',
        tags:''
    });

    a({
        id:22,
        title:"FileExtensions",
        content:"FileExtensions",
        description:'',
        tags:''
    });

    a({
        id:23,
        title:"ProjectPath",
        content:"ProjectPath",
        description:'',
        tags:''
    });

    a({
        id:24,
        title:"BuildTarget",
        content:"BuildTarget",
        description:'',
        tags:''
    });

    a({
        id:25,
        title:"DotNetCoreTestExtensions",
        content:"DotNetCoreTestExtensions",
        description:'',
        tags:''
    });

    a({
        id:26,
        title:"FilePathExtensions",
        content:"FilePathExtensions",
        description:'',
        tags:''
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/ProjectParserExtensions',
        title:"ProjectParserExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/SolutionParserExtensions',
        title:"SolutionParserExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/CustomProjectParserResult',
        title:"CustomProjectParserResult",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/GlobbingExtensions',
        title:"GlobbingExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/AssertExtensions',
        title:"AssertExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/FileSystemExtensions',
        title:"FileSystemExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/BuildTargetExecutable',
        title:"BuildTargetExecutable",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/ProjectPathExtensions',
        title:"ProjectPathExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/PackageReference',
        title:"PackageReference",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/EnumerableExtensions',
        title:"EnumerableExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/ProjectTypes',
        title:"ProjectTypes",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/StringExtensions',
        title:"StringExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/LoggingExtensions',
        title:"LoggingExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/ProjectType',
        title:"ProjectType",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/RuntimeOptions',
        title:"RuntimeOptions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/DirectoryExtensions',
        title:"DirectoryExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/DotNetBuildSettingsExtensions',
        title:"DotNetBuildSettingsExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/DotNetCliToolReference',
        title:"DotNetCliToolReference",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/NetCoreProject',
        title:"NetCoreProject",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/CustomProjectFile',
        title:"CustomProjectFile",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/XDocumentExtensions',
        title:"XDocumentExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/EnvironmentExtensions',
        title:"EnvironmentExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/FileExtensions',
        title:"FileExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/ProjectPath',
        title:"ProjectPath",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/BuildTarget',
        title:"BuildTarget",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/DotNetCoreTestExtensions',
        title:"DotNetCoreTestExtensions",
        description:""
    });

    y({
        url:'/Cake.Incubator/Cake.Incubator/api/Cake.Incubator/FilePathExtensions',
        title:"FilePathExtensions",
        description:""
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
