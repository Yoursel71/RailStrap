import Image from "next/image";
import { siteAsset } from "@/lib/site";

export function ArtworkBand() {
  return (
    <section className="relative overflow-hidden py-14 sm:py-20">
      <div className="mx-auto max-w-5xl px-4">
        <div className="relative overflow-hidden rounded-2xl border border-white/10 shadow-[0_30px_90px_-45px_rgba(124,92,255,.6)]">
          <Image
            src={siteAsset("/railbit-artwork.jpg")}
            alt="Illustrated Roblox characters beside the railbit logo"
            width={2000}
            height={1126}
            sizes="(min-width: 1024px) 960px, 92vw"
            className="h-auto w-full"
            draggable={false}
          />
          <div className="pointer-events-none absolute inset-0 ring-1 ring-inset ring-white/10" />
        </div>

        <p className="mx-auto mt-5 max-w-[52ch] text-center text-sm leading-6 text-muted-foreground">
          A personal project, not a product. Every feature exists because something was missing on
          a real machine, on a real connection, mid-session.
        </p>
      </div>
    </section>
  );
}
