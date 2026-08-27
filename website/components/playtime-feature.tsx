import Image from "next/image";
import { Feature } from "@/components/ui/feature-with-image";
import { siteAsset } from "@/lib/site";

export function PlaytimeFeature() {
  return (
    <Feature
      badge="Playtime Stats"
      title="See where your hours actually went"
      description="RailStrap keeps a private log of how long you've played each game, right on your own device. Nothing is uploaded — it's just there when you're curious, in Settings → Playtime Stats."
      visual={
        <div className="relative h-full w-full">
          <Image
            src={siteAsset("/screenshots/playtime-stats.png")}
            alt="RailStrap Settings, Playtime Stats page, showing the Track playtime toggle enabled"
            fill
            sizes="(min-width: 1024px) 700px, 90vw"
            className="object-cover"
          />
        </div>
      }
    />
  );
}
