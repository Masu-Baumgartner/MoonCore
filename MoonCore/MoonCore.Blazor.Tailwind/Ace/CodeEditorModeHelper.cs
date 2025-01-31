namespace MoonCore.Blazor.Tailwind.Ace;

public static class CodeEditorModeHelper
{
    // We probably will never need every of this modes ;)
    private static readonly Dictionary<string, string[]> ExtensionIndex = new()
    {
        { "abap", ["abap"] },
        { "abc", ["abc"] },
        { "actionscript", ["as"] },
        { "ada", ["ada", "adb"] },
        { "alda", ["alda"] },
        { "apache_conf", ["htaccess", "htgroups", "htpasswd", "conf", "htaccess", "htgroups", "htpasswd"] },
        { "apex", ["apex", "cls", "trigger", "tgr"] },
        { "aql", ["aql"] },
        { "asciidoc", ["asciidoc", "adoc"] },
        { "asl", ["dsl", "asl", "asl.json"] },
        { "assembly_x86", ["asm", "a"] },
        { "astro", ["astro"] },
        { "autohotkey", ["ahk"] },
        { "batchfile", ["bat", "cmd"] },
        { "bibtex", ["bib"] },
        { "c_cpp", ["cpp", "c", "cc", "cxx", "h", "hh", "hpp", "ino"] },
        { "c9search", ["c9search_results"] },
        { "cirru", ["cirru", "cr"] },
        { "clojure", ["clj", "cljs"] },
        { "cobol", ["cbl", "cob"] },
        { "coffee", ["coffee", "cf", "cson", "cakefile"] },
        { "coldfusion", ["cfm", "cfc"] },
        { "crystal", ["cr"] },
        { "csharp", ["cs"] },
        { "csound_document", ["csd"] },
        { "csound_orchestra", ["orc"] },
        { "csound_score", ["sco"] },
        { "css", ["css"] },
        { "curly", ["curly"] },
        { "cuttlefish", ["conf"] },
        { "d", ["d", "di"] },
        { "dart", ["dart"] },
        { "diff", ["diff", "patch"] },
        { "django", ["djt", "html.djt", "dj.html", "djhtml"] },
        { "dockerfile", ["dockerfile"] },
        { "dot", ["dot"] },
        { "drools", ["drl"] },
        { "edifact", ["edi"] },
        { "eiffel", ["e", "ge"] },
        { "ejs", ["ejs"] },
        { "elixir", ["ex", "exs"] },
        { "elm", ["elm"] },
        { "erlang", ["erl", "hrl"] },
        { "flix", ["flix"] },
        { "forth", ["frt", "fs", "ldr", "fth", "4th"] },
        { "fortran", ["f", "f90"] },
        { "fsharp", ["fsi", "fs", "ml", "mli", "fsx", "fsscript"] },
        { "fsl", ["fsl"] },
        { "ftl", ["ftl"] },
        { "gcode", ["gcode"] },
        { "gherkin", ["feature"] },
        { "gitignore", [".gitignore"] },
        { "glsl", ["glsl", "frag", "vert"] },
        { "gobstones", ["gbs"] },
        { "golang", ["go"] },
        { "graphqlschema", ["gql"] },
        { "groovy", ["groovy"] },
        { "haml", ["haml"] },
        { "handlebars", ["hbs", "handlebars", "tpl", "mustache"] },
        { "haskell", ["hs"] },
        { "haskell_cabal", ["cabal"] },
        { "haxe", ["hx"] },
        { "hjson", ["hjson"] },
        { "html", ["html", "htm", "xhtml", "vue", "we", "wpy"] },
        { "html_elixir", ["eex", "html.eex"] },
        { "html_ruby", ["erb", "rhtml", "html.erb"] },
        { "ini", ["ini", "conf", "cfg", "prefs"] },
        { "io", ["io"] },
        { "ion", ["ion"] },
        { "jack", ["jack"] },
        { "jade", ["jade", "pug"] },
        { "java", ["java"] },
        { "javascript", ["js", "jsm", "jsx", "cjs", "mjs"] },
        { "jexl", ["jexl"] },
        { "json", ["json"] },
        { "json5", ["json5"] },
        { "jsoniq", ["jq"] },
        { "jsp", ["jsp"] },
        { "jssm", ["jssm", "jssm_state"] },
        { "jsx", ["jsx"] },
        { "julia", ["jl"] },
        { "kotlin", ["kt", "kts"] },
        { "latex", ["tex", "latex", "ltx", "bib"] },
        { "latte", ["latte"] },
        { "less", ["less"] },
        { "liquid", ["liquid"] },
        { "lisp", ["lisp"] },
        { "livescript", ["ls"] },
        { "log", ["log"] },
        { "logiql", ["logic", "lql"] },
        { "logtalk", ["lgt"] },
        { "lsl", ["lsl"] },
        { "lua", ["lua"] },
        { "luapage", ["lp"] },
        { "lucene", ["lucene"] },
        { "makefile", ["makefile", "gnumakefile", "makefile", "ocamlmakefile", "make"] },
        { "markdown", ["md", "markdown"] },
        { "mask", ["mask"] },
        { "matlab", ["matlab"] },
        { "maze", ["mz"] },
        { "mediawiki", ["wiki", "mediawiki"] },
        { "mel", ["mel"] },
        { "mips", ["s", "asm"] },
        { "mixal", ["mixal"] },
        { "mushcode", ["mc", "mush"] },
        { "mysql", ["mysql"] },
        { "nasal", ["nas"] },
        { "nginx", ["nginx", "conf"] },
        { "nim", ["nim"] },
        { "nix", ["nix"] },
        { "nsis", ["nsi", "nsh"] },
        { "nunjucks", ["nunjucks", "nunjs", "nj", "njk"] },
        { "objectivec", ["m", "mm"] },
        { "ocaml", ["ml", "mli"] },
        { "odin", ["odin"] },
        { "partiql", ["partiql", "pql"] },
        { "pascal", ["pas", "p"] },
        { "perl", ["pl", "pm"] },
        { "pgsql", ["pgsql"] },
        { "php", ["php", "inc", "phtml", "shtml", "php3", "php4", "php5", "phps", "phpt", "aw", "ctp", "module"] },
        { "php_laravel_blade", ["blade.php"] },
        { "pig", ["pig"] },
        { "plsql", ["plsql"] },
        { "powershell", ["ps1"] },
        { "praat", ["praat", "praatscript", "psc", "proc"] },
        { "prisma", ["prisma"] },
        { "prolog", ["plg", "prolog"] },
        { "properties", ["properties"] },
        { "protobuf", ["proto"] },
        { "prql", ["prql"] },
        { "puppet", ["epp", "pp"] },
        { "python", ["py"] },
        { "qml", ["qml"] },
        { "r", ["r"] },
        { "raku", ["raku", "rakumod", "rakutest", "p6", "pl6", "pm6"] },
        { "razor", ["cshtml", "asp"] },
        { "rdoc", ["rd"] },
        { "red", ["red", "reds"] },
        { "rhtml", ["rhtml"] },
        { "robot", ["robot", "resource"] },
        { "rst", ["rst"] },
        { "ruby", ["rb", "ru", "gemspec", "rake", "guardfile", "rakefile", "gemfile"] },
        { "rust", ["rs"] },
        { "sac", ["sac"] },
        { "sass", ["sass"] },
        { "scad", ["scad"] },
        { "scala", ["scala", "sbt"] },
        { "scheme", ["scm", "sm", "rkt", "oak", "scheme"] },
        { "scrypt", ["scrypt"] },
        { "scss", ["scss"] },
        { "sh", ["sh", "bash", ".bashrc"] },
        { "sjs", ["sjs"] },
        { "slim", ["slim", "skim"] },
        { "smarty", ["smarty", "tpl"] },
        { "smithy", ["smithy"] },
        { "snippets", ["snippets"] },
        { "soy_template", ["soy"] },
        { "space", ["space"] },
        { "sparql", ["rq"] },
        { "sql", ["sql"] },
        { "sqlserver", ["sqlserver"] },
        { "stylus", ["styl", "stylus"] },
        { "svg", ["svg"] },
        { "swift", ["swift"] },
        { "tcl", ["tcl"] },
        { "terraform", ["tf", "tfvars", "terragrunt"] },
        { "tex", ["tex"] },
        { "text", ["txt"] },
        { "textile", ["textile"] },
        { "toml", ["toml"] },
        { "tsx", ["tsx"] },
        { "turtle", ["ttl"] },
        { "twig", ["twig", "swig"] },
        { "typescript", ["ts", "mts", "cts", "typescript", "str"] },
        { "vala", ["vala"] },
        { "vbscript", ["vbs", "vb"] },
        { "velocity", ["vm"] },
        { "verilog", ["v", "vh", "sv", "svh"] },
        { "vhdl", ["vhd", "vhdl"] },
        { "visualforce", ["vfp", "component", "page"] },
        { "wollok", ["wlk", "wpgm", "wtest"] },
        { "xml", ["xml", "rdf", "rss", "wsdl", "xslt", "atom", "mathml", "mml", "xul", "xbl", "xaml"] },
        { "xquery", ["xq"] },
        { "yaml", ["yaml", "yml"] },
        { "zeek", ["zeek", "bro"] }
    };

    public static string GetModeFromFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).Replace(".", "");

        if (string.IsNullOrEmpty(extension))
            return "text";

        foreach (var entry in ExtensionIndex)
        {
            if (entry.Value.Any(x => string.Equals(x, extension, StringComparison.InvariantCultureIgnoreCase)))
                return entry.Key;
        }

        return "text";
    }
}