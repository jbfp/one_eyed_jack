using System.Data;
using static Dapper.SqlMapper;

namespace Sequence.Postgres
{
    internal sealed class GameIdTypeHandler : TypeHandler<GameId>
    {
        public override GameId Parse(object value) => new((Guid)value);
        public override void SetValue(IDbDataParameter parameter, GameId value) => parameter.Value = value.Value;
    }

    internal sealed class PlayerHandleTypeHandler : StringTypeHandler<PlayerHandle>
    {
        protected override string Format(PlayerHandle xml) => xml.Value;
        protected override PlayerHandle Parse(string xml) => new(xml);
    }

    internal sealed class PlayerIdTypeHandler : TypeHandler<PlayerId>
    {
        public override PlayerId Parse(object value) => new((int)value);
        public override void SetValue(IDbDataParameter parameter, PlayerId value) => parameter.Value = value.Value;
    }

    internal sealed class SeedTypeHandler : TypeHandler<Seed>
    {
        public override Seed Parse(object value) => new((int)value);
        public override void SetValue(IDbDataParameter parameter, Seed value) => parameter.Value = value.ToInt32();
    }
}
