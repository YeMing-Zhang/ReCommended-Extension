﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.Await
{
    public class AwaitForMethods
    {
        async Task Method()
        {
            await Task.Delay(10);
            await Task.Delay(20);
        }

        async Task Method2()
        {
            if (Environment.UserInteractive)
            {
                await Task.Delay(10);
            }

            await Task.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        }

        async Task Method3() => await Task.Delay(10);

        async Task<int> Method4()
        {
            if (Environment.UserInteractive)
            {
                return await Task.FromResult(3);
            }

            await Task.Delay(10);
            return await Task.FromResult(4);
        }

        async Task<int> Method5()
        {
            await Task.Delay(10);
            return await Task.FromResult(3);

            int LocalFunction() => 4;
        }

        async Task Method6() => await Task.FromResult(3);

        async Task<int> Method7() => await Task.FromResult(3);

        async Task<int> Method_AwaitNonLast()
        {
            await Task.Delay(10);
            await Task.Delay(20);

            return 3;
        }

        async Task Method2_AwaitNonLast()
        {
            using (new Process())
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }
        }

        async Task<int> Method3_AwaitNonLast()
        {
            await Task.Delay(10);
            var result = await Task.FromResult(3);
            return result;
        }

        async Task Method_AwaitNonTask()
        {
            await Task.Delay(10);
            await Task.Yield();
        }

        async Task Method_WithConfigureAwait()
        {
            await Task.Delay(10).ConfigureAwait(false);
        }

        async Task Method_WithConfigureAwait_AsExpressionBodied() => await Task.Delay(10).ConfigureAwait(false);

        async Task<int> Method_NestedInUsingScope()
        {
            using (new Process())
            {
                return await Task.FromResult(3);
            }
        }

        async Task<int> Method_NestedInUsingScope(int x)
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }
        }

        async Task<int> Method_UsingDeclaration()
        {
            using var p = new Process();

            return await Task.FromResult(3);
        }

        async Task<int> Method_UsingDeclaration(int x)
        {
            using var p = new Process();

            if (x > 2)
            {
                return await Task.FromResult(3);
            }
        }

        async Task<int> Method_NestedInTryBlock()
        {
            try
            {
                return await Task.FromResult(3);
            }
            catch
            {
                throw;
            }
        }

        async Task<int> Method_NestedInTryBlock(int x)
        {
            try
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }
            catch
            {
                throw;
            }
        }
    }

    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<Task> Method = async () =>
            {
                await Task.Delay(10);
                await Task.Delay(20);
            };

            Func<Task> Method2 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    await Task.Delay(10);
                }

                await Task.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            };

            Func<Task> Method3 = async () => await Task.Delay(10);

            Func<Task<int>> Method4 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    return await Task.FromResult(3);
                }

                await Task.Delay(10);
                return await Task.FromResult(4);
            };

            Func<Task<int>> Method5 = async () =>
            {
                await Task.Delay(10);
                return await Task.FromResult(3);

                int LocalFunction() => 4;
            };

            Func<Task> Method6 = async () => await Task.FromResult(3);

            Func<Task<int>> Method7 = async () => await Task.FromResult(3);

            Func<Task<int>> Method_AwaitNonLast = async () =>
            {
                await Task.Delay(10);
                await Task.Delay(20);

                return 3;
            };

            Func<Task> Method2_AwaitNonLast = async () =>
            {
                using (new Process())
                {
                    await Task.Delay(10);
                    await Task.Delay(20);
                }
            };

            Func<Task<int>> Method3_AwaitNonLast = async () =>
            {
                await Task.Delay(10);
                var result = await Task.FromResult(3);
                return result;
            };

            Func<Task> Method_AwaitNonTask = async () =>
            {
                await Task.Delay(10);
                await Task.Yield();
            };

            Func<Task> Method_WithConfigureAwait = async () => { await Task.Delay(10).ConfigureAwait(false); };

            Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async () => await Task.Delay(10).ConfigureAwait(false);

            Func<Task<int>> Method_NestedInUsingScope = async () =>
            {
                using (new Process())
                {
                    return await Task.FromResult(3);
                }
            };

            Func<Task<int>> Method_NestedInUsingScope = async (int x) =>
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }
            };

            Func<Task<int>> Method_UsingDeclaration = async () =>
            {
                using var p = new Process();

                return await Task.FromResult(3);
            };

            Func<Task<int>> Method_UsingDeclaration = async (int x) =>
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            };

            Func<Task<int>> Method_NestedInTryBlock = async () =>
            {
                try
                {
                    return await Task.FromResult(3);
                }
                catch
                {
                    throw;
                }
            };

            Func<Task<int>> Method_NestedInTryBlock = async (int x) =>
            {
                try
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }
                catch
                {
                    throw;
                }
            };
        }
    }

    public class AwaitForLambdaFields
    {
        Func<Task> Method = async () =>
        {
            await Task.Delay(10);
            await Task.Delay(20);
        };

        Func<Task> Method2 = async () =>
        {
            if (Environment.UserInteractive)
            {
                await Task.Delay(10);
            }

            await Task.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        };

        Func<Task> Method3 = async () => await Task.Delay(10);

        Func<Task<int>> Method4 = async () =>
        {
            if (Environment.UserInteractive)
            {
                return await Task.FromResult(3);
            }

            await Task.Delay(10);
            return await Task.FromResult(4);
        };

        Func<Task<int>> Method5 = async () =>
        {
            await Task.Delay(10);
            return await Task.FromResult(3);

            int LocalFunction() => 4;
        };

        Func<Task> Method6 = async () => await Task.FromResult(3);

        Func<Task<int>> Method7 = async () => await Task.FromResult(3);

        Func<Task<int>> Method_AwaitNonLast = async () =>
        {
            await Task.Delay(10);
            await Task.Delay(20);

            return 3;
        };

        Func<Task> Method2_AwaitNonLast = async () =>
        {
            using (new Process())
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }
        };

        Func<Task<int>> Method3_AwaitNonLast = async () =>
        {
            await Task.Delay(10);
            var result = await Task.FromResult(3);
            return result;
        };

        Func<Task> Method_AwaitNonTask = async () =>
        {
            await Task.Delay(10);
            await Task.Yield();
        };

        Func<Task> Method_WithConfigureAwait = async () => { await Task.Delay(10).ConfigureAwait(false); };

        Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async () => await Task.Delay(10).ConfigureAwait(false);

        Func<Task<int>> Method_NestedInUsingScope = async () =>
        {
            using (new Process())
            {
                return await Task.FromResult(3);
            }
        };

        Func<Task<int>> Method_NestedInUsingScope = async (int x) =>
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }
        };

        Func<Task<int>> Method_UsingDeclaration = async () =>
        {
            using var p = new Process();

            return await Task.FromResult(3);
        };

        Func<Task<int>> Method_UsingDeclaration = async (int x) =>
        {
            using var p = new Process();

            if (x > 2)
            {
                return await Task.FromResult(3);
            }
        };

        Func<Task<int>> Method_NestedInTryBlock = async () =>
        {
            try
            {
                return await Task.FromResult(3);
            }
            catch
            {
                throw;
            }
        };

        Func<Task<int>> Method_NestedInTryBlock = async (int x) =>
        {
            try
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }
            catch
            {
                throw;
            }
        };
    }

    public class AwaitForAnonymousMethodVariables
    {
        void Method()
        {
            Func<Task> Method = async delegate
            {
                await Task.Delay(10);
                await Task.Delay(20);
            };

            Func<Task> Method2 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    await Task.Delay(10);
                }

                await Task.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            };

            Func<Task> Method3 = async delegate { await Task.Delay(10); };

            Func<Task<int>> Method4 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    return await Task.FromResult(3);
                }

                await Task.Delay(10);
                return await Task.FromResult(4);
            };

            Func<Task<int>> Method5 = async delegate
            {
                await Task.Delay(10);
                return await Task.FromResult(3);

                int LocalFunction() => 4;
            };

            Func<Task> Method6 = async delegate { await Task.FromResult(3); };

            Func<Task<int>> Method7 = async delegate { return await Task.FromResult(3); };

            Func<Task<int>> Method_AwaitNonLast = async delegate
            {
                await Task.Delay(10);
                await Task.Delay(20);

                return 3;
            };

            Func<Task> Method2_AwaitNonLast = async delegate
            {
                using (new Process())
                {
                    await Task.Delay(10);
                    await Task.Delay(20);
                }
            };

            Func<Task<int>> Method3_AwaitNonLast = async delegate
            {
                await Task.Delay(10);
                var result = await Task.FromResult(3);
                return result;
            };

            Func<Task> Method_AwaitNonTask = async delegate
            {
                await Task.Delay(10);
                await Task.Yield();
            };

            Func<Task> Method_WithConfigureAwait = async delegate { await Task.Delay(10).ConfigureAwait(false); };

            Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async delegate { await Task.Delay(10).ConfigureAwait(false); };

            Func<Task<int>> Method_NestedInUsingScope = async delegate
            {
                using (new Process())
                {
                    return await Task.FromResult(3);
                }
            };

            Func<Task<int>> Method_NestedInUsingScope = async delegate(int x)
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }
            };

            Func<Task<int>> Method_UsingDeclaration = async delegate
            {
                using var p = new Process();

                return await Task.FromResult(3);
            };

            Func<Task<int>> Method_UsingDeclaration = async delegate(int x)
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            };

            Func<Task<int>> Method_NestedInTryBlock = async delegate
            {
                try
                {
                    return await Task.FromResult(3);
                }
                catch
                {
                    throw;
                }
            };

            Func<Task<int>> Method_NestedInTryBlock = async delegate(int x)
            {
                try
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }
                catch
                {
                    throw;
                }
            };
        }
    }

    public class AwaitForAnonymousMethodFields
    {
        Func<Task> Method = async delegate
        {
            await Task.Delay(10);
            await Task.Delay(20);
        };

        Func<Task> Method2 = async delegate
        {
            if (Environment.UserInteractive)
            {
                await Task.Delay(10);
            }

            await Task.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        };

        Func<Task> Method3 = async delegate { await Task.Delay(10); };

        Func<Task<int>> Method4 = async delegate
        {
            if (Environment.UserInteractive)
            {
                return await Task.FromResult(3);
            }

            await Task.Delay(10);
            return await Task.FromResult(4);
        };

        Func<Task<int>> Method5 = async delegate
        {
            await Task.Delay(10);
            return await Task.FromResult(3);

            int LocalFunction() => 4;
        };

        Func<Task> Method6 = async delegate { await Task.FromResult(3); };

        Func<Task<int>> Method7 = async delegate { return await Task.FromResult(3); };

        Func<Task<int>> Method_AwaitNonLast = async delegate
        {
            await Task.Delay(10);
            await Task.Delay(20);

            return 3;
        };

        Func<Task> Method2_AwaitNonLast = async delegate
        {
            using (new Process())
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }
        };

        Func<Task<int>> Method3_AwaitNonLast = async delegate
        {
            await Task.Delay(10);
            var result = await Task.FromResult(3);
            return result;
        };

        Func<Task> Method_AwaitNonTask = async delegate
        {
            await Task.Delay(10);
            await Task.Yield();
        };

        Func<Task> Method_WithConfigureAwait = async delegate { await Task.Delay(10).ConfigureAwait(false); };

        Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async delegate { await Task.Delay(10).ConfigureAwait(false); };

        Func<Task<int>> Method_NestedInUsingScope = async delegate
        {
            using (new Process())
            {
                return await Task.FromResult(3);
            }
        };

        Func<Task<int>> Method_NestedInUsingScope = async delegate(int x)
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }
        };

        Func<Task<int>> Method_UsingDeclaration = async delegate
        {
            using var p = new Process();

            return await Task.FromResult(3);
        };

        Func<Task<int>> Method_UsingDeclaration = async delegate(int x)
        {
            using var p = new Process();

            if (x > 2)
            {
                return await Task.FromResult(3);
            }
        };

        Func<Task<int>> Method_NestedInTryBlock = async delegate
        {
            try
            {
                return await Task.FromResult(3);
            }
            catch
            {
                throw;
            }
        };

        Func<Task<int>> Method_NestedInTryBlock = async delegate(int x)
        {
            try
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }
            catch
            {
                throw;
            }
        };
    }

    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task Method()
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }

            async Task Method2()
            {
                if (Environment.UserInteractive)
                {
                    await Task.Delay(10);
                }

                await Task.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            }

            async Task Method3() => await Task.Delay(10);

            async Task<int> Method4()
            {
                if (Environment.UserInteractive)
                {
                    return await Task.FromResult(3);
                }

                await Task.Delay(10);
                return await Task.FromResult(4);
            }

            async Task<int> Method5()
            {
                await Task.Delay(10);
                return await Task.FromResult(3);

                int LocalFunction() => 4;
            }

            async Task Method6() => await Task.FromResult(3);

            async Task<int> Method7() => await Task.FromResult(3);

            async Task<int> Method_AwaitNonLast()
            {
                await Task.Delay(10);
                await Task.Delay(20);

                return 3;
            }

            async Task Method2_AwaitNonLast()
            {
                using (new Process())
                {
                    await Task.Delay(10);
                    await Task.Delay(20);
                }
            }

            async Task<int> Method3_AwaitNonLast()
            {
                await Task.Delay(10);
                var result = await Task.FromResult(3);
                return result;
            }

            async Task Method_AwaitNonTask()
            {
                await Task.Delay(10);
                await Task.Yield();
            }

            async Task Method_WithConfigureAwait()
            {
                await Task.Delay(10).ConfigureAwait(false);
            }

            async Task Method_WithConfigureAwait_AsExpressionBodied() => await Task.Delay(10).ConfigureAwait(false);

            async Task<int> Method_NestedInUsingScope()
            {
                using (new Process())
                {
                    return await Task.FromResult(3);
                }
            }

            async Task<int> Method_NestedInUsingScope(int x)
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }
            }

            async Task<int> Method_UsingDeclaration()
            {
                using var p = new Process();

                return await Task.FromResult(3);
            }

            async Task<int> Method_UsingDeclaration(int x)
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }

            async Task<int> Method_NestedInTryBlock()
            {
                try
                {
                    return await Task.FromResult(3);
                }
                catch
                {
                    throw;
                }
            }

            async Task<int> Method_NestedInTryBlock(int x)
            {
                try
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}