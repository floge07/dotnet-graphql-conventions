using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.NewtonsoftJson;
using Tests.Templates;
using Tests.Templates.Extensions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Tests.Execution
{
    public class SchemaExecutionTests : ConstructionTestBase
    {
        [Test]
        public async Task Can_Have_Decimals_In_Schema()
        {
            var schema = Schema<SchemaTypeWithDecimal>();
            schema.ShouldHaveQueries(1);
            schema.ShouldHaveMutations(0);
            schema.Query.ShouldHaveFieldWithName("test");
            var result = await schema.ExecuteAsync((e) => e.Query = "query { test }");
            ResultHelpers.AssertNoErrorsInResult(result);
        }

        private class SchemaTypeWithDecimal
        {
            public QueryTypeWithDecimal Query { get; }
        }

        private class QueryTypeWithDecimal
        {
            public decimal Test => 10;
        }

        [Test]
        public async Task Test_NonNull_Validation()
        {
            string query = @"
                query {
                    test {
                        children {
                            field(problematic: [""1"", ""2""])
                        }
                    }
                }
            ";

            var engine = GraphQLEngine.New();

            engine.WithQuery<NonNullTests.QueryType>();

            var executor = engine
                .NewExecutor()
                .WithQueryString(query);

            var result1 = await executor.ExecuteAsync();
            System.Diagnostics.Debug.WriteLine(new GraphQLSerializer(indent: true).Serialize(result1));


            var schema = Schema<NonNullTests>();
            schema.ShouldHaveQueries(1);
            schema.ShouldHaveMutations(0);
            schema.Query.ShouldHaveFieldWithName("test");

            var result2 = await schema.ExecuteAsync((e) => e.Query = query);
            System.Diagnostics.Debug.WriteLine(result2);
            ResultHelpers.AssertNoErrorsInResult(result2);
        }

        public class NonNullTests
        {
            public QueryType Query { get; }

            public class QueryType
            {
                public NonNull<DataType> test()
                {
                    return new DataType()
                    {
                        children = new List<NonNull<DataType>> {
                        new DataType(),
                        new DataType()
                    }
                    };
                }

                public class DataType
                {
                    public NonNull<List<NonNull<DataType>>> children { get; set; }

                    public List<NonNull<string>> field(NonNull<List<string>> problematic)
                    {
                        return new List<NonNull<string>> {
                            new NonNull<string>("ab"),
                            new NonNull<string>("cd")
                        };
                    }
                }
            }
        }
    }
}
