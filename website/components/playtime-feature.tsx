import { Feature } from "@/components/ui/feature-with-image";
import { PlaytimeStatsMockup } from "@/components/playtime-stats-mockup";

export function PlaytimeFeature() {
  return (
    <Feature
      badge="Playtime Stats"
      title="See where your hours actually went"
      description="RailStrap keeps a private log of how long you've played each game, right on your own device. Nothing is uploaded — it's just there when you're curious, in Settings → Playtime Stats."
      visual={<PlaytimeStatsMockup />}
    />
  );
}
