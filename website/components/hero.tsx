"use client";

import { ArrowDownToLine, ShieldCheck } from "lucide-react";
import { ContainerScroll } from "@/components/ui/container-scroll-animation";
import { AppMockup } from "@/components/app-mockup";
import { Button } from "@/components/ui/button";

export function Hero() {
  return (
    <section id="hero" className="relative overflow-hidden">
      <div className="pointer-events-none absolute -left-[120px] -top-[220px] h-[520px] w-[520px] rounded-full bg-[#6d3fff] opacity-[0.28] blur-[100px]" />
      <div className="pointer-events-none absolute -right-[160px] -top-[140px] h-[460px] w-[460px] rounded-full bg-[#3f7bff] opacity-[0.28] blur-[100px]" />

      <ContainerScroll
        titleComponent={
          <div className="flex flex-col items-center">
            <span className="mb-5 inline-flex items-center gap-2 rounded-full border border-[#7c5cff]/30 bg-[#7c5cff]/10 py-1.5 pl-2 pr-3 text-xs font-semibold text-[#c3b6ff]">
              <span className="h-1.5 w-1.5 rounded-full bg-[#8a5bff] shadow-[0_0_0_3px_rgba(124,92,255,.25)]" />
              Free, open source, Windows
            </span>

            <h1 className="text-4xl font-extrabold leading-[1.05] tracking-tight text-foreground md:text-[54px]">
              A better way to
              <br />
              launch{" "}
              <span className="bg-gradient-to-br from-[#8a5bff] to-[#4f8cff] bg-clip-text text-transparent">
                Roblox
              </span>
              .
            </h1>

            <p className="mx-auto mt-5 max-w-[46ch] text-[16.5px] text-muted-foreground">
              RailStrap is a third-party bootstrapper for Roblox — a drop-in replacement for the
              default launcher that adds the features Roblox won&apos;t, without touching your
              data.
            </p>

            <div className="mt-7 flex flex-wrap items-center justify-center gap-3">
              <Button asChild>
                <a href="https://github.com/Yoursel71/RailStrap/releases/latest" target="_blank" rel="noopener noreferrer">
                  <ArrowDownToLine /> Download for Windows
                </a>
              </Button>
              <Button variant="ghost" asChild>
                <a href="#features">Browse features</a>
              </Button>
            </div>

            <p className="mt-4 flex items-center justify-center gap-1.5 text-[12.5px] text-zinc-500">
              <ShieldCheck className="h-3.5 w-3.5" />
              Only official source is this GitHub repo &middot; requires .NET 6 Desktop Runtime
            </p>
          </div>
        }
      >
        <AppMockup />
      </ContainerScroll>
    </section>
  );
}
