using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokeApiNet.Data;
using PokeApiNet.Models;

namespace PokePlanner.Util
{
    /// <summary>
    /// Singleton for caching data during the application session.
    /// </summary>
    public class SessionCache
    {
        /// <summary>
        /// The selected version group.
        /// </summary>
        public VersionGroup VersionGroup;

        /// <summary>
        /// The selected version group's generation.
        /// </summary>
        public Generation Generation;

        /// <summary>
        /// The selected version group's HM moves.
        /// </summary>
        public IList<Move> HMMoves;

        /// <summary>
        /// Singleton constructor;
        /// </summary>
        private SessionCache() { }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static SessionCache Instance { get; set; } = new SessionCache();

        /// <summary>
        /// Client for PokeApi.
        /// </summary>
        public static PokeApiClient Client { get; } = new PokeApiClient();

        /// <summary>
        /// Wrapper for <see cref="PokeApiClient.GetResourceAsync{T}(int)"/> with exception handling.
        /// Returns null if an exception was thrown.
        /// </summary>
        public static async Task<T> Get<T>(int id) where T : ResourceBase
        {
            T res;
            try
            {
                res = await Client.GetResourceAsync<T>(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($@"Get() failed to retrieve {typeof(T)} resource by id.");
                return null;
            }

            return res;
        }

        /// <summary>
        /// Wrapper for <see cref="PokeApiClient.GetResourceAsync{T}(string)"/> with exception handling.
        /// Returns null if an exception was thrown.
        /// </summary>
        public static async Task<T> Get<T>(string name) where T : ResourceBase
        {
            T res;
            try
            {
                res = await Client.GetResourceAsync<T>(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($@"Get() failed to retrieve {typeof(T)} resource by name.");
                return null;
            }

            return res;
        }

        /// <summary>
        /// Wrapper for <see cref="PokeApiClient.GetResourceAsync{T}(UrlNavigation{T})"/> with exception handling.
        /// Returns null if an exception was thrown.
        /// </summary>
        public static async Task<T> Get<T>(UrlNavigation<T> nav) where T : ResourceBase
        {
            T res;
            try
            {
                res = await Client.GetResourceAsync(nav);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($@"Get() failed to retrieve {typeof(T)} resource from UrlNavigation object.");
                return null;
            }

            return res;
        }

        /// <summary>
        /// Wrapper for <see cref="PokeApiClient.GetResourceAsync{T}(IEnumerable{UrlNavigation{T}})"/> with exception handling.
        /// Returns null if an exception was thrown.
        /// </summary>
        public static async Task<IList<T>> Get<T>(IEnumerable<UrlNavigation<T>> nav) where T : ResourceBase
        {
            List<T> resList;
            try
            {
                resList = await Client.GetResourceAsync(nav);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($@"Get() failed to retrieve {typeof(T)} resources from UrlNavigation objects.");
                return null;
            }

            return resList;
        }

        /// <summary>
        /// Wrapper for <see cref="PokeApiClient.GetNamedResourcePageAsync{T}()"/> with exception handling.
        /// Returns null if an exception was thrown.
        /// </summary>
        public static async Task<NamedApiResourceList<T>> GetPage<T>() where T : NamedApiResource
        {
            NamedApiResourceList<T> resList;
            try
            {
                resList = await Client.GetNamedResourcePageAsync<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($@"Get() failed to retrieve page of {typeof(T)} resources.");
                return null;
            }

            return resList;
        }
    }
}
