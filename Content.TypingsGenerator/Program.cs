// Copyright (C) 2025 Igor Spichkin

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Attribute = System.Attribute;
using Vector3 = Robust.Shared.Maths.Vector3;


namespace Content.TypingsGenerator;


internal sealed class ReflectionManager
{
    private readonly List<Assembly> _assemblies = [];
    private readonly List<System.Type> _getAllTypesCache = [];

    public ReflectionManager()
    {
        _assemblies.Add(typeof(Robust.Shared.IoC.IoCManager).Assembly);
        _assemblies.Add(typeof(Robust.Client.IBaseClient).Assembly);
        _assemblies.Add(typeof(Robust.Server.IBaseServer).Assembly);
        _assemblies
            .Add(typeof(Shared.Entry.EntryPoint).Assembly);
        _assemblies
            .Add(typeof(Client.Entry.EntryPoint).Assembly);
        _assemblies
            .Add(typeof(Server.Entry.EntryPoint).Assembly);
    }

    public IEnumerable<System.Type> GetAllChildren<T>(bool inclusive = false) => GetAllChildren(typeof(T), inclusive);

    public IEnumerable<System.Type> GetAllChildren(System.Type baseType, bool inclusive = false)
    {
        EnsureGetAllTypesCache();

        foreach (var type in _getAllTypesCache)
        {
            if (!baseType.IsAssignableFrom(type) || type.IsAbstract)
                continue;

            if (baseType == type && !inclusive)
                continue;

            yield return type;
        }
    }

    private void EnsureGetAllTypesCache()
    {
        if (_getAllTypesCache.Count != 0)
            return;

        var totalLength = 0;
        var typeSets = new List<System.Type[]>();

        foreach (var assembly in _assemblies)
        {
            var types = assembly.GetTypes();
            typeSets.Add(types);
            totalLength += types.Length;
        }

        _getAllTypesCache.Capacity = totalLength;

        foreach (var typeSet in typeSets)
        {
            foreach (var type in typeSet)
            {
                var attribute = (ReflectAttribute?) Attribute.GetCustomAttribute(type, typeof(ReflectAttribute));

                if (!(attribute?.Discoverable ?? ReflectAttribute.DEFAULT_DISCOVERABLE))
                    continue;

                _getAllTypesCache.Add(type);
            }
        }
    }
}

internal sealed class AdHocComponentClass;

internal sealed class AdHocPrototypeClass;

internal sealed class AdHocTaggedClass;

internal static class MetaTypes
{
    public static readonly Dictionary<System.Type, (string Name, string? Code)> AdHocTypes = new()
    {
        {
            typeof(AdHocComponentClass),
            ("Component",
                """
                abstract class Component {
                    fixed type: String
                }
                """)
        },
        {
            typeof(AdHocPrototypeClass),
            ("Prototype",
                """
                abstract class Prototype {
                    fixed type: String
                    id: String
                }
                """
            )
        },
        {
            typeof(AdHocTaggedClass),
            ("TaggedClass", "abstract class TaggedClass {}")
        },
        {
            typeof(EntityUid),
            ("EntityUid", "typealias EntityUid = String")
        },
        {
            typeof(ProtoId<>),
            ("ProtoId", "typealias ProtoId<T> = String")
        },
        {
            typeof(EntProtoId),
            ("ProtoId", null)
        },
        {
            typeof(EntProtoId<>),
            ("ProtoId", null)
        },
        {
            typeof(LocId),
            ("LocId", "typealias LocId = String")
        },
        {
            typeof(ResPath),
            ("ResPath",
                """
                const function UriEncode(s: String): String = s.replaceAll(" ", "%20").replaceAll("'", "%27")

                typealias ResPath = String(read("file:Resources\(UriEncode(this))") != null)
                """
            )
        },
        {
            typeof(Vector2i),
            ("Vector2i",
                """
                const function Vector2i(_x: Int, _y: Int): Vector2i = new Vector2i {
                    x = _x
                    y = _y
                }

                class Vector2i {
                    x: Int
                    y: Int
                }
                """
            )
        },
        {
            typeof(Vector2),
            ("Vector2",
                """
                const function Vector2(_x: Float, _y: Float): Vector2 = new Vector2 {
                    x = _x
                    y = _y
                }

                class Vector2 {
                    x: Float
                    y: Float
                }
                """
            )
        },
        {
            typeof(Vector3),
            ("Vector3",
                """
                const function Vector3(_x: Float, _y: Float, _z: Float): Vector3 = new Vector3 {
                    x = _x
                    y = _y
                    z = _z
                }

                class Vector3 {
                    x: Float
                    y: Float
                    z: Float
                }
                """
            )
        },
        {
            typeof(Box2),
            ("Box2",
                """
                const function Box2(_left: Float, _bottom: Float, _right: Float, _top: Float): Box2 = new Box2 {
                    left = _left
                    bottom = _bottom
                    right = _right
                    top = _top
                }

                class Box2 {
                    left: Float
                    bottom: Float
                    right: Float
                    top: Float
                }
                """
            )
        },
        {
            typeof(Box2i),
            ("Box2i",
                """
                const function Box2i(_left: Int, _bottom: Int, _right: Int, _top: Int): Box2i = new Box2i {
                    left = _left
                    bottom = _bottom
                    right = _right
                    top = _top
                }

                class Box2i {
                    left: Int
                    bottom: Int
                    right: Int
                    top: Int
                }
                """
            )
        },
        {
            typeof(Color),
            ("Color",
                """
                local const function ToHexPadded(number: Number, padding: UInt): String = number.toRadixString(16).padStart(padding, "0")

                local const function IsHexColor(hex: String): Boolean = hex.startsWith("#") && (hex.length == 7 || hex.length == 9)

                const function ColorRGB(r: UInt8, g: UInt8, b: UInt8): Color = ColorRGBA(r, g, b, 255)

                const function ColorRGBA(r: UInt8, g: UInt8, b: UInt8, a: UInt8): Color = "#\(ToHexPadded(r, 2))\(ToHexPadded(g, 2))\(ToHexPadded(b, 2))\(ToHexPadded(a, 2))"

                typealias Color = String(IsHexColor(this))
                """
            )
        },
        {
            typeof(Angle),
            ("Angle", "typealias Angle = Float")
        },
        {
            typeof(FixedPoint2),
            ("FixedPoint2", "typealias FixedPoint2 = Float")
        },
        {
            typeof(SpriteComponent),
            ("SpriteComponent",
                """
                typealias DrawDepth = "LowFloors"
                    |"ThickPipe"
                    |"ThickWire"
                    |"ThinPipe"
                    |"ThinWire"
                    |"BelowFloor"
                    |"FloorTiles"
                    |"FloorObjects"
                    |"Puddles"
                    |"HighFloorObjects"
                    |"DeadMobs"
                    |"SmallMobs"
                    |"Walls"
                    |"WallTops"
                    |"Objects"
                    |"SmallObjects"
                    |"WallMountedItems"
                    |"Items"
                    |"BelowMobs"
                    |"Mobs"
                    |"OverMobs"
                    |"Doors"
                    |"BlastDoors"
                    |"Overdoors"
                    |"Effects"
                    |"Ghosts"
                    |"Overlays"

                typealias LayerRenderingStrategy = "Default"
                    |"SnapToCardinals"
                    |"NoRotation"
                    |"UseSpriteStrategy"

                class SpriteComponent extends Component {
                    fixed type: String = "Sprite"
                    netsync: Boolean?
                    granularLayersRendering: Boolean?
                    visible: Boolean?
                    drawdepth: DrawDepth?
                    scale: Vector2?
                    rotation: Angle?
                    offset: Vector2?
                    color: Color?
                    sprite: String?
                    layers: Listing<PrototypeLayerData>?
                    state: String?
                    texture: String?
                    overrideContainerOcclusion: Boolean?
                    getScreenTexture: Boolean?
                    raiseShaderEvent: Boolean?
                    noRot: Boolean?
                    snapCardinals: Boolean?
                    overrideDir: Direction?
                    enableOverrideDir: Boolean?
                }
                """
            )
        },
        {
            typeof(ComponentRegistry),
            ("ComponentRegistry", "typealias ComponentRegistry = Listing<Component>(isDistinctBy((c) -> c.type))")
        },
        {
            typeof(Fixture),
            ("Fixture",
                """
                class Fixture {
                  shape: IPhysShape?
                  friction: Float?
                  restitution: Float?
                  hard: Boolean?
                  density: Float?
                  layer: Set<CollisionGroup>?
                  mask: Set<CollisionGroup>?
                }
                """
            )
        },
        {
            typeof(SpriteSpecifier),
            ("SpriteSpecifier",
                """
                typealias SpriteSpecifier = ResPath|Rsi
                """
            )
        }
    };

    public static readonly Dictionary<System.Type, Func<Type>> BuiltinTypes = new()
    {
        { typeof(char), () => new("Char", false, null) },
        { typeof(string), () => new("String", false, null) },
        { typeof(TimeSpan), () => new("String", false, null) },
        { typeof(bool), () => new("Boolean", false, null) },
        { typeof(short), () => new("Int16", false, null) },
        { typeof(ushort), () => new("UInt16", false, null) },
        { typeof(int), () => new("Int32", false, null) },
        { typeof(uint), () => new("UInt32", false, null) },
        { typeof(float), () => new("Float", false, null) },
        { typeof(double), () => new("Float", false, null) },
        { typeof(HashSet<>), () => new("Set", false, null) },
        { typeof(ImmutableHashSet<>), () => new("Set", false, null) },
        { typeof(Dictionary<,>), () => new("Mapping", false, null) },
        { typeof(ImmutableDictionary<,>), () => new("Mapping", false, null) },
        { typeof(ReadOnlyDictionary<,>), () => new("Mapping", false, null) },
        { typeof(FrozenDictionary<,>), () => new("Mapping", false, null) },
        { typeof(List<>), () => new("Listing", false, null) },
        { typeof(IReadOnlyList<>), () => new("Listing", false, null) },
        { typeof(ImmutableList<>), () => new("Listing", false, null) },
        { typeof(IReadOnlyCollection<>), () => new("Listing", false, null) }
    };
}

internal record struct Type
{
    public string Name;

    public bool IsNullable;

    public string? Module;

    public List<Type> TypeArguments = [];

    public Type(
        string name,
        bool isNullable,
        string? module,
        List<Type>? typeArguments = null
    )
    {
        Name = name;
        IsNullable = isNullable;
        Module = module;
        TypeArguments = typeArguments ?? [];
    }

    public string Stringify()
    {
        var sb = new StringBuilder();

        if (Module is not null)
            sb.Append($"{Module}.");

        sb.Append(Name);

        if (TypeArguments.Count != 0)
        {
            sb.Append('<');

            for (var i = 0; i < TypeArguments.Count; i++)
            {
                var type = TypeArguments[i];

                sb.Append(type.Stringify());
                sb.Append(i != TypeArguments.Count - 1 ? ',' : '>');
            }
        }

        if (IsNullable)
            sb.Append('?');

        return sb.ToString();
    }
}

public enum FieldModifier
{
    None,
    Fixed
}

internal record struct FieldDefinition
{
    public string Name;

    public Type Type;

    public FieldModifier Modifier;

    public string? DefaultValue;

    public FieldDefinition(
        string name,
        Type type,
        FieldModifier modifier = FieldModifier.None,
        string? defaultValue = null
    )
    {
        Name = name;
        Type = type;
        Modifier = modifier;
        DefaultValue = defaultValue;
    }

    public string Stringify()
    {
        var sb = new StringBuilder();

        switch (Modifier)
        {
            case FieldModifier.None:
                break;
            case FieldModifier.Fixed:
                sb.Append("fixed ");

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        sb.Append($"{Name}: {Type.Stringify()}");

        if (DefaultValue is { } defaultValue)
            sb.Append($" = {defaultValue}");

        return sb.ToString();
    }
}

internal interface ITypeDefinition
{
    public string Name { get; }

    public string Stringify();
}

internal record struct EnumDefinition : ITypeDefinition
{
    public string Name { get; }

    public List<string> Variants = [];

    public EnumDefinition(string name, List<string> variants)
    {
        Name = name;
        Variants = variants;
    }

    public string Stringify()
    {
        var sb = new StringBuilder();

        sb.Append($"typealias {Name} = ");

        for (var i = 0; i < Variants.Count; i++)
        {
            sb.Append($"\"{Variants[i]}\"");

            if (i != Variants.Count - 1)
                sb.Append(" | ");
        }

        sb.AppendLine();

        return sb.ToString();
    }
}

internal enum ClassModifier : byte
{
    None,
    Abstract,
    Open
}

internal record struct ClassDefinition : ITypeDefinition
{
    public string Name { get; }

    public List<FieldDefinition> Fields = [];

    public Type? Base;

    public ClassModifier Modifier;

    public ClassDefinition(
        string name,
        List<FieldDefinition> fields,
        Type? baseClass = null,
        ClassModifier? modifier = null
    )
    {
        Name = name;
        Fields = fields;
        Base = baseClass;
        Modifier = modifier ?? ClassModifier.Open;
    }

    public string Stringify()
    {
        var sb = new StringBuilder();

        switch (Modifier)
        {
            case ClassModifier.None:
                break;
            case ClassModifier.Abstract:
                sb.Append("abstract ");

                break;
            case ClassModifier.Open:
                sb.Append("open ");

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        sb.Append($"class {Name} ");

        if (Base is { } baseClass)
            sb.Append($"extends {baseClass.Stringify()} ");

        sb.AppendLine("{");

        foreach (var field in Fields)
        {
            sb.Append("  ");
            sb.AppendLine(field.Stringify());
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    public void MergeFields(ClassDefinition b)
    {
        foreach (var field in b.Fields)
        {
            if (Fields.Any(f => f.Name == field.Name))
                continue;

            Fields.Add(field);
        }
    }
}

internal record struct ModuleDefinition
{
    public string Name;

    public List<string> Imports;

    public List<ClassDefinition> ClassDefinitions;

    public ModuleDefinition(string name, List<string> imports, List<ClassDefinition> classDefinitions)
    {
        Name = name;
        Imports = imports;
        ClassDefinitions = classDefinitions;
    }

    public string Stringify()
    {
        var sb = new StringBuilder();

        foreach (var import in Imports)
            sb.AppendLine($"import \"{import}\"");

        if (Imports.Count > 0)
            sb.AppendLine();

        foreach (var def in ClassDefinitions)
            sb.AppendLine(def.Stringify());

        return sb.ToString();
    }
}

internal static class Program
{
    private const string PklFolder = "Pkl";
    private const string PklTypingsFile = $"{PklFolder}/Typings.pkl";

    private static readonly Dictionary<System.Type, ITypeDefinition> GlobalTypes = [];
    private static readonly HashSet<string> ReservedWords = ["hidden", "abstract", "open", "delete", "true", "false",];
    private static readonly ReflectionManager ReflectionManager = new();

    private static async Task<int> Main()
    {
        if (Path.GetFileName(Environment.CurrentDirectory) == $"{nameof(Content)}.{nameof(TypingsGenerator)}")
            Environment.CurrentDirectory = Path.GetFullPath(Path.Join(Environment.CurrentDirectory, "../../"));

        if (!Directory.Exists(Path.Join(Environment.CurrentDirectory, PklFolder)))
        {
            Console.WriteLine("The folder 'Resources/Prototype' not found");

            return -1;
        }

        if (File.Exists(PklTypingsFile))
            File.Delete(PklTypingsFile);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var classes = GenerateTypings(ReflectionManager);

        Console.WriteLine($"Typings generated in {(int) stopwatch.Elapsed.TotalMilliseconds} ms.");

        var sb = new StringBuilder();

        sb.AppendLine("// AUTO GENERATED\n");
        sb.AppendLine("import \"base.pkl\"");
        sb.AppendLine();
        sb.AppendLine("// AD HOC TYPES");

        foreach (var adhoc in MetaTypes.AdHocTypes)
        {
            if (string.IsNullOrEmpty(adhoc.Value.Code))
                continue;

            sb.AppendLine(adhoc.Value.Code);
            sb.AppendLine();
        }

        sb.AppendLine("// GENERATED TYPINGS");

        foreach (var c in classes)
            sb.AppendLine(c.Value.Stringify());

        HashSet<string> saved = [];

        foreach (var kvp in GlobalTypes)
        {
            if (saved.Contains(kvp.Value.Name) || classes.ContainsKey(kvp.Value.Name))
                continue;

            sb.AppendLine(kvp.Value.Stringify());
            saved.Add(kvp.Value.Name);
        }

        sb.AppendLine(
            """
            // END

            content: Listing<Prototype|EntityPrototype>

            local const function RenderTaggedClass(v: TaggedClass): Any = new Mapping {
                [new RenderDirective { text = "!type" }] = new RenderDirective { text = v.getClass().simpleName }
                ...v.toMap()
            }

            output {
                value = content
                renderer = new YamlRenderer {
                    converters {
                        [Vector2] = (v) -> "\(v.x),\(v.y)"
                        [Vector2i] = (v) -> "\(v.x),\(v.y)"
                        [Vector3] = (v) -> "\(v.x),\(v.y),\(v.z)"
                        [Box2] = (v) -> "\(v.left),\(v.bottom),\(v.right),\(v.top)"
                        [Box2i] = (v) -> "\(v.left),\(v.bottom),\(v.right),\(v.top)"
                        [TaggedClass] = (v) -> RenderTaggedClass(v)
                    }
                }
            }
            """
        );

        await File.WriteAllTextAsync(PklTypingsFile, sb.ToString());

        return 0;
    }

    private static Dictionary<string, ClassDefinition> GenerateTypings(
        ReflectionManager reflectionManager
    )
    {
        var classes = new Dictionary<string, ClassDefinition>();

        foreach (var prototype in reflectionManager.GetAllChildren<IPrototype>())
        {
            if (prototype.FullName!.StartsWith("Robust.UnitTesting"))
                continue;

            if (TryToMetaType(prototype) is not null)
                continue;

            var typeValue = CalculatePrototypeTypeName(prototype);

            var classDef = GenerateClassDefinition(
                    prototype,
                    [
                        new(
                            "type",
                            new("String", false, null),
                            FieldModifier.Fixed,
                            $"\"{typeValue}\"")
                    ]) with
                {
                    Base = new Type("Prototype", false, null)
                };

            if (classes.TryGetValue(classDef.Name, out var oldDef))
            {
                oldDef.MergeFields(classDef);
                classes[classDef.Name] = oldDef;
            }
            else
                classes.Add(classDef.Name, classDef);

            if (GlobalTypes.TryGetValue(prototype, out var oldDef2) && oldDef2 is ClassDefinition oldClassDef)
            {
                oldClassDef.MergeFields(classDef);
                classes[classDef.Name] = oldClassDef;
            }
            else
                GlobalTypes.Add(prototype, classDef);
        }

        foreach (var component in reflectionManager.GetAllChildren<IComponent>())
        {
            if (component.FullName!.StartsWith("Robust.UnitTesting"))
                continue;

            if (TryToMetaType(component) is not null)
                continue;

            var typeValue = CalculateComponentName(component);

            var classDef = GenerateClassDefinition(
                    component,
                    [
                        new(
                            "type",
                            new("String", false, null),
                            FieldModifier.Fixed,
                            $"\"{typeValue}\"")
                    ]) with
                {
                    Base = new Type("Component", false, null)
                };

            if (classes.TryGetValue(classDef.Name, out var oldDef))
            {
                oldDef.MergeFields(classDef);
                classes[classDef.Name] = oldDef;
            }
            else
                classes.Add(classDef.Name, classDef);

            if (GlobalTypes.TryGetValue(component, out var oldDef2) && oldDef2 is ClassDefinition oldClassDef)
            {
                oldClassDef.MergeFields(classDef);
                classes[classDef.Name] = oldClassDef;
            }
            else
                GlobalTypes.Add(component, classDef);
        }

        return classes;
    }

    private static ClassDefinition GenerateClassDefinition(
        System.Type t,
        List<FieldDefinition>? baseFields = null
    )
    {
        var fields = new Dictionary<string, FieldDefinition>();

        foreach (var field in baseFields ?? [])
            fields.TryAdd(field.Name, field);

        foreach (var prop in t.GetAllProperties())
        {
            if (prop.HasCustomAttribute<IncludeDataFieldAttribute>())
            {
                var inlinedFields = GenerateClassDefinition(prop.PropertyType).Fields;

                foreach (var inlinedField in inlinedFields)
                    fields.TryAdd(inlinedField.Name, inlinedField);

                continue;
            }

            if (GenerateFieldDefinition(t, prop) is { } def)
                fields.TryAdd(def.Name, def);
        }

        foreach (var field in t.GetAllFields())
        {
            if (field.HasCustomAttribute<IncludeDataFieldAttribute>())
            {
                var inlinedFields = GenerateClassDefinition(field.FieldType).Fields;

                foreach (var inlinedField in inlinedFields)
                    fields.TryAdd(inlinedField.Name, inlinedField);

                continue;
            }

            if (GenerateFieldDefinition(t, field) is { } def)
                fields.TryAdd(def.Name, def);
        }

        return new(t.Name, fields.Values.ToList());
    }

    private static EnumDefinition GenerateEnumDefinition(System.Type t)
    {
        var variants = new List<string>();

        foreach (var variant in t.GetEnumNames())
            variants.Add(variant);

        return new(t.Name, variants);
    }

    private static FieldDefinition? GenerateFieldDefinition(System.Type t, MemberInfo info)
    {
        if (info.HasCustomAttribute<JsonIgnoreAttribute>() ||
            info.HasCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>())
            return null;

        System.Type type;

        if (info is PropertyInfo propertyInfo)
            type = propertyInfo.PropertyType;
        else if (info is FieldInfo fieldInfo)
            type = fieldInfo.FieldType;
        else
            return null;

        if (info.GetCustomAttribute<DataFieldAttribute>() is not { } dataFieldAttribute)
            return null;

        var fieldName = GetFieldName(info, dataFieldAttribute);

        var isNullable = !dataFieldAttribute.Required;

        var def = TryToMetaType(type) ?? GenerateTypeDefinition(type);

        if (def is null)
        {
            Console.WriteLine(
                $"Couldn't make the typings for the field '{type.FullName} {type.Name}' of type '{t.FullName}'");

            def = new("Any", false, null);
        }

        def = def.Value with { IsNullable = isNullable, };

        return new(fieldName, def.Value);
    }

    private static Type? GenerateTypeDefinition(System.Type t)
    {
        if (t.IsGenericType)
            return null;

        if (t == typeof(Enum))
            return null;

        Type? def;

        if (GlobalTypes.TryGetValue(t, out var globalType))
            def = new Type(globalType.Name, false, null);
        else
        {
            if (t is { IsEnum: false, IsAbstract: false, IsInterface: false, })
            {
                if (!t.HasCustomAttribute<DataDefinitionAttribute>() &&
                    !t.HasCustomAttribute<SerializableAttribute>() && !t.IsDefined(
                        typeof(ImplicitDataDefinitionForInheritorsAttribute),
                        true))
                    return null;
            }

            if (t.IsAbstract || t.IsInterface)
            {
                var newClass = GenerateClassDefinition(t) with
                {
                    Modifier = ClassModifier.Abstract,
                    Base = new Type("TaggedClass", false, null)
                };

                GlobalTypes.Add(t, newClass);

                var newClassType = new Type(newClass.Name, false, null);

                foreach (var child in ReflectionManager.GetAllChildren(t))
                {
                    if (child.IsGenericType)
                        continue;

                    if (GlobalTypes.TryGetValue(child, out var value))
                    {
                        var childClass = (ClassDefinition) value;

                        GlobalTypes[child] = childClass with { Base = newClassType, };

                        continue;
                    }

                    if (TryToMetaType(child) is not null)
                        continue;

                    var childDef = GenerateClassDefinition(child) with
                    {
                        Base = newClassType,
                        Modifier = child.IsAbstract || child.IsInterface ? ClassModifier.Abstract : ClassModifier.None
                    };

                    GlobalTypes.TryAdd(child, childDef);
                }

                return newClassType;
            }

            if (t.IsEnum)
            {
                var newEnum = GenerateEnumDefinition(t);

                def = new Type(newEnum.Name, false, null);

                GlobalTypes.Add(t, newEnum);
            }
            else
            {
                // Stack overflow saver
                GlobalTypes.Add(t, new ClassDefinition(t.Name, []));

                var newClass = GenerateClassDefinition(t);

                if (t.IsAssignableTo(typeof(IPrototype)))
                    newClass = newClass with { Base = new Type("Prototype", false, null), };
                else if (t.IsAssignableTo(typeof(Component)))
                    newClass = newClass with { Base = new Type("Component", false, null), };

                def = new Type(newClass.Name, false, null);

                GlobalTypes[t] = newClass;
            }
        }

        return def;
    }

    private static string GetFieldName(MemberInfo fieldInfo, DataFieldAttribute dataField)
    {
        string name;

        if (dataField.Tag is { } tag)
            name = tag;
        else
            name = JsonNamingPolicy.CamelCase.ConvertName(fieldInfo.Name);

        return ReservedWords.Contains(name) ? $"`{name}`" : name;
    }

    private static string CalculatePrototypeTypeName(System.Type type)
    {
        if (type.GetCustomAttribute<PrototypeAttribute>() is { } prototypeAttribute &&
            prototypeAttribute.Type is { } typeOverride)
            return typeOverride;

        const string prototype = "Prototype";

        if (!type.Name.EndsWith(prototype))
            throw new InvalidPrototypeNameException($"Prototype {type} must end with the word Prototype");

        var name = type.Name.AsSpan();
        return $"{char.ToLowerInvariant(name[0])}{name[1..^prototype.Length]}";
    }

    private static string CalculateComponentName(System.Type type)
    {
        // Attributes can use any name they want, they are for bypassing the automatic names
        // If a parent class has this attribute, a child class will use the same name, unless it also uses this attribute
        if (Attribute.GetCustomAttribute(type, typeof(ComponentProtoNameAttribute)) is ComponentProtoNameAttribute
            attribute)
            return attribute.PrototypeName;

        const string component = "Component";
        var typeName = type.Name;
        if (!typeName.EndsWith(component))
            throw new InvalidComponentNameException($"Component {type} must end with the word Component");

        var name = typeName[..^component.Length];

        const string client = "Client";
        const string server = "Server";
        const string shared = "Shared";

        if (typeName.StartsWith(client, StringComparison.Ordinal))
            name = typeName[client.Length..^component.Length];
        else if (typeName.StartsWith(server, StringComparison.Ordinal))
            name = typeName[server.Length..^component.Length];
        else if (typeName.StartsWith(shared, StringComparison.Ordinal))
            name = typeName[shared.Length..^component.Length];

        DebugTools.Assert(name != string.Empty, $"Component {type} has invalid name {type.Name}");

        return name;
    }

    public static Type? TryToMetaType(System.Type t, bool skipGenericCheck = false)
    {
        if (!skipGenericCheck)
        {
            if (t.IsGenericType)
            {
                var genericDef = t.GetGenericTypeDefinition();

                if (genericDef == typeof(Nullable<>))
                {
                    var ty = TryToMetaType(t.GenericTypeArguments[0]);

                    if (ty.HasValue)
                        ty = ty.Value with { IsNullable = true, };

                    return ty;
                }

                if (TryToMetaType(genericDef, true) is not { } type)
                    return null;

                foreach (var p in t.GenericTypeArguments)
                {
                    var pType = TryToMetaType(p) ?? GenerateTypeDefinition(p);

                    if (pType is null)
                    {
                        Console.WriteLine(
                            $"Couldn't make the typings for the generic type argument '{p.FullName} {p.Name}' in type '{t.FullName}'");

                        type.TypeArguments.Add(new("Any", false, null));

                        continue;
                    }

                    type.TypeArguments.Add(pType.Value);
                }

                return type;
            }

            if (t.IsArray)
            {
                var elementType = t.GetElementType()!;
                var type = TryToMetaType(elementType) ?? GenerateTypeDefinition(elementType);

                if (type is null)
                    return null;

                return new Type("Listing", false, null, [type.Value,]);
            }
        }

        if (MetaTypes.BuiltinTypes.TryGetValue(t, out var builtinType))
            return builtinType();

        if (MetaTypes.AdHocTypes.TryGetValue(t, out var adHocType))
            return new Type(adHocType.Name, false, null);

        return null;
    }
}
