using UnityEngine;
using Unity.Entities;
using System.Linq;
using Unity.Collections;
using System;

namespace ZhTool.Entities
{
    public class ABlobConfigGeneral<TConfig, TBlob> : MonoBehaviour
    where TConfig : unmanaged, IConfigComponent<TBlob>
    where TBlob : unmanaged
    {
        public void Bake<T>(Baker<T> baker) where T : ABlobConfigGeneral<TConfig, TBlob>
        {
            var blobRef = CreatePool(baker);
            TConfig config = new();
            config.SetBlobRef(blobRef);

            // Register the blob asset to the baker for de-duplication and reverting 
            baker.AddBlobAsset(ref blobRef, out var hash);

            var entity = baker.GetEntity(TransformUsageFlags.None);
            baker.AddComponent(entity, config);
        }

        private BlobAssetReference<TBlob> CreatePool<T>(Baker<T> baker) where T : ABlobConfigGeneral<TConfig, TBlob>
        {
            // Get blob handle.
            var builder = new BlobBuilder(Allocator.Temp);
            ref var blob = ref builder.ConstructRoot<TBlob>();

            TransferData(ref blob, ref builder, baker);

            // Build blob.
            var result = builder.CreateBlobAssetReference<TBlob>(Allocator.Persistent);
            builder.Dispose();
            return result;
        }

        protected virtual void TransferData<T>(ref TBlob blob, ref BlobBuilder builder, Baker<T> baker) where T : ABlobConfigGeneral<TConfig, TBlob>
        {

        }
    }
}
